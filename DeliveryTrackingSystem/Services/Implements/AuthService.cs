using AutoMapper;
using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Dtos.Auth;
using DeliveryTrackingSystem.Models.Dtos.User;
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

        public async Task<string> AssignRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email) ??
                throw new Exception("User not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new Exception("This Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to assign role: {errors}");
            }

            return $"Role '{roleName}' assigned to user '{email}' successfully";
        }

        public async Task<string> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new Exception("User not found");

            var newPassword = GenerateRandomPassword();

            // إعادة تعيين الباسورد
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                throw new Exception("Password reset failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // إرسال الباسورد الجديدة بالإيميل
            var subject = "Password Reset";
            var message = $"Your new password is: {newPassword}";
            await _emailService.SendEmailAsync(user.Email, subject, message);

            return "A new password has been sent to your email.";
        }

        //public async Task<string> ResetPasswordAsync(ResetPasswordDto model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email) ??
        //       throw new Exception("User not found");

        //    //  التحقق من الرمز
        //    if (user.ResetCode != model.ResetCode)
        //        throw new Exception("Invalid reset code");

        //    // التحقق مما إذا كانت كلمة المرور الجديدة هي نفس القديمة
        //    var passwordCheck = await _userManager.CheckPasswordAsync(user, model.NewPassword);
        //    if (passwordCheck)
        //        throw new Exception("New password cannot be the same as the current password.");

        //    //  إعادة تعيين كلمة المرور
        //    var resetPassword = await _userManager.GeneratePasswordResetTokenAsync(user);

        //    var result = await _userManager.ResetPasswordAsync(user, resetPassword, model.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        //        throw new Exception($"Failed to reset password: {errors}");
        //    }

        //    //  إزالة الرمز بعد الاستخدام
        //    user.ResetCode = null;
        //    await _userManager.UpdateAsync(user);

        //    return "Password has been reset successfully!";
        //}

        public async Task<string> Send2FACodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ??
                throw new Exception("User not found");

            // التحقق مما إذا كان هناك كود موجود ولكنه منتهي الصلاحية
            if (user.TwoFactorCodeExpiration != null && user.TwoFactorCodeExpiration > DateTime.Now)
                return "A valid 2FA code has already been sent. Please check your email.";

            var twoFactorCode = GenerateCode();

            user.TwoFactorCode = twoFactorCode;
            user.TwoFactorCodeExpiration = DateTime.Now.AddMinutes(5);

            await _userManager.UpdateAsync(user);

            var subject = "Your 2FA Code";
            var message = $"Your Two-Factor Authentication code is : {twoFactorCode}";
            await _emailService.SendEmailAsync(user.Email, subject, message);

            return "A 2FA code has been sent to your email.";
        }

        public async Task<string> Resend2FACodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ??
                throw new Exception("User not found");

            if (user.TwoFactorSentAt != null && (DateTime.UtcNow - user.TwoFactorSentAt.Value).TotalSeconds < 60)
            {
                throw new Exception("Please wait at least 1 minute before requesting a new code.");
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new Exception($"Your account is locked. Try again at {user.LockoutEnd.Value.ToLocalTime()}.");
            }

            // إعادة ضبط المحاولات إذا مرت ساعة
            if (user.LastTwoFactorAttempt != null && (DateTime.UtcNow - user.LastTwoFactorAttempt.Value).TotalHours >= 1)
            {
                user.TwoFactorAttempts = 0;
            }

            if (user.TwoFactorAttempts >= 5)
            {
                throw new Exception("You have exceeded the maximum number of attempts. Please try again later.");
            }

            var newCode = GenerateCode();
            user.TwoFactorCode = newCode;
            user.TwoFactorCodeExpiration = DateTime.UtcNow.AddMinutes(10);

            user.TwoFactorSentAt = DateTime.UtcNow;

            user.TwoFactorAttempts += 1;
            user.LastTwoFactorAttempt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            await _emailService.SendEmailAsync(user.Email, "Your 2FA Code", $"Your new 2FA code is: {newCode}");

            return "A new 2FA code has been sent to your email.";
        }

        public async Task<string> Verify2FACodeAsync(Verify2FACodeDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email)
             ?? throw new Exception("User not found");

            // التحقق مما إذا كان الكود قد انتهت صلاحيته
            if (user.TwoFactorCode == null || user.TwoFactorCodeExpiration < DateTime.UtcNow)
                throw new Exception("The 2FA code has expired. Please request a new one.");

            // التحقق من صحة الكود
            if (user.TwoFactorCode != model.Code)
            {
                user.FailedTwoFactorAttempts++;

                // إذا تجاوز 5 محاولات خاطئة، يتم قفل الحساب
                if (user.FailedTwoFactorAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(10); //  قفل الحساب لمدة 10 دقيقة
                    await _userManager.UpdateAsync(user);
                    throw new Exception("Too many failed attempts. Your account is locked for 10 minutes.");
                }

                await _userManager.UpdateAsync(user);
                throw new Exception("Invalid 2FA code.");
            }

            // Reset the 2FA code after successful verification
            user.FailedTwoFactorAttempts = 0;
            user.TwoFactorAttempts = 0;
            user.LockoutEnd = null;
            user.TwoFactorCode = null;
            user.TwoFactorCodeExpiration = null;

            await _userManager.UpdateAsync(user);

            return "2FA verification successful";
        }

        public async Task<string> UnlockUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email) ??
                throw new Exception("User not found");

            //  التحقق مما إذا كان الحساب مقفلًا حاليًا
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                user.LockoutEnd = null; // إزالة القفل
                user.FailedTwoFactorAttempts = 0; // إعادة تعيين المحاولات الفاشلة
                await _userManager.UpdateAsync(user);
                return "User account has been unlocked successfully.";
            }

            return "User account is not locked.";
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email)
             ?? throw new Exception("User not found");

            // Verify old password
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isPasswordCorrect)
                throw new Exception("Incorrect Current password");

            // Ensure new password is not the same as the old one
            if (model.CurrentPassword == model.NewPassword)
                throw new Exception("New password cannot be the same as the Current password");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Password change failed: {errors}");
            }

            return "Password changed successfully";
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            if (roles == null || !roles.Any())
                throw new Exception("No Roles Exist!");

            return roles;
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName) ??
                throw new Exception("Role not found");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to delete role: {errors}");
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                throw new Exception("Role does not exist");

            var users = await _userManager.GetUsersInRoleAsync(roleName) ??
                throw new Exception("Users does not exist");

            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);

            // User لكل  Role لتحديد 
            foreach (var userDto in usersDto)
            {
                var user = users.FirstOrDefault(u => u.Id == userDto.ApplicationUserId);
                userDto.Roles = await _userManager.GetRolesAsync(user);
            }

            return usersDto;
        }

        public async Task UpdateRoleAsync(UpdateRoleDto model)
        {
            var role = await _roleManager.FindByNameAsync(model.OldRoleName) ??
                throw new Exception("Role not found");

            var roleExists = await _roleManager.RoleExistsAsync(model.NewRoleName);
            if (roleExists)
                throw new Exception("New role name already exists");

            role.Name = model.NewRoleName;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update role: {errors}");
            }
        }

        public async Task AddRoleAsync(string roleName)
        {
            // التحقق مما إذا كان الدور موجودًا بالفعل
            if (await _roleManager.RoleExistsAsync(roleName))
                throw new Exception("Role already exists.");

            var concurrencyStamp = Guid.NewGuid().ToString(); // إنشاء قيمة `ConcurrencyStamp` 

            var role = new IdentityRole
            {
                Id = concurrencyStamp,
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                ConcurrencyStamp = concurrencyStamp
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create role: {errors}");
            }
        }

        public async Task<IEnumerable<string>> GetRolesByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Email is required");
            }

            var user = await _userManager.FindByEmailAsync(email) ??
               throw new Exception("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task RemoveUserFromRoleAsync(string email, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
                throw new Exception("Email and role are required.");

            var user = await _userManager.FindByEmailAsync(email) ??
                throw new Exception("User not found");

            if (!await _userManager.IsInRoleAsync(user, role))
                throw new Exception("User is not in this Role!");

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
                throw new Exception("Failed to remove user from role");
        }

        public async Task LogoutAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail) ??
                throw new Exception("User Not Founded!");

            foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
            {
                token.RevokedOn = DateTime.UtcNow;
            }

            await _userManager.UpdateAsync(user);
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

        private string GenerateCode()
        {
            Random random = new Random();
            return random.Next(10000000, 99999999).ToString();
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
