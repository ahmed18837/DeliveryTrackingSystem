using AutoMapper;
using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Dtos.Auth;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.Interfaces;
using DeliveryTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DeliveryTrackingSystem.Services.Implements
{
    public class AuthService(UserManager<ApplicationUser> manager
              , RoleManager<IdentityRole> roleManager, IMapper mapper, IOptions<JwtOptions> jwt, IEmailService emailService, IFileService fileService, IUserRepository userRepository) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = manager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IFileService _fileService = fileService;
        private readonly IMapper _mapper = mapper;
        private readonly JwtOptions _jwt = jwt.Value;

        public async Task AdminRegisterAsync(RequestRegisterDto request)
        {
            await GenericRegisterAsync(request, "Admin");
        }

        public async Task EmployeeRegisterAsync(RequestRegisterDto request)
        {
            await GenericRegisterAsync(request, "Employee");
        }

        public async Task DriverRegisterAsync(RequestRegisterDto request)
        {
            await GenericRegisterAsync(request, "Driver");
        }

        public async Task<ResponseDto> LoginAsync(LoginDto request)
        {
            var authModel = new ResponseDto();

            var user = await _userManager.FindByEmailAsync(request.Email) ??
                throw new Exception("UserName or Password is incorrect");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
                throw new Exception("UserName or Password is incorrect");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0)
                throw new Exception("User has no assigned roles");

            var token = await CreateJwtToken(user);
            string encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            authModel.AccessToken = encodedToken;

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role)); // تم التعديل هنا

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
    }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private async Task GenericRegisterAsync(RequestRegisterDto request, string role)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Input data cannot be null.");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("A user with this email already exists.");

            // Check if phone number already exists
            var existingPhoneUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingPhoneUser != null)
                throw new Exception("A user with this phone number already exists.");

            // 1️ Create user in AspNetUsers
            var appUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(appUser, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {errors}");
            }

            await _userManager.AddToRoleAsync(appUser, role);

            // 2️ Create User entity and link with ApplicationUser
            var user = _mapper.Map<User>(request);

            user.ApplicationUserId = appUser.Id;

            var folderPath = Path.Combine("Users", role); // Users/Admin, Users/Employee...
            user.ProfileImageFileName = _fileService.SaveFile(request.Image, folderPath);
            user.Role = role;

            try
            {
                await _userRepository.AddAsync(user);

                // 3️ Send welcome email
                var htmlBody = $@"
                    <h2>Welcome to Delivery Tracking System</h2>
                    <p>Dear <strong>{request.FullName}</strong>,</p>
                    <p>Your account has been successfully created as a <strong>{role}</strong>.</p>
                    <p>You can now log in using your email: <strong>{request.Email}</strong></p>
                    <br/>
                    <p>Thank you,<br/>Delivery Tracking System Team</p>
                    ";

                await _emailService.SendEmailAsync(request.Email, "Welcome to Delivery Tracking System", htmlBody);

            }
            catch (DbUpdateException ex)
            {
                await _userManager.DeleteAsync(appUser); // Rollback
                throw new Exception($"Database update failed: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser); // Rollback
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(5),
                CreatedOn = DateTime.UtcNow
            };
        }

    }
}
