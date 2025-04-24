using AutoMapper;
using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Models.Dtos.Auth;
using DeliveryTrackingSystem.Models.Dtos.User;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.Interfaces;
using DeliveryTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingSystem.Services.Implements
{
    public class UserService(IUserRepository userRepository, IMapper mapper, IFileService fileService,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IFileService _fileService = fileService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IEmailService _emailService = emailService;

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (!users.Any() || users == null) throw new Exception("No users found!");
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : throw new Exception("User not found!");
        }

        public async Task CreateAsync(RegisterDto userCreateDto)
        {
            if (userCreateDto == null)
                throw new ArgumentNullException(nameof(userCreateDto), "Input data cannot be null.");

            var existingUser = await _userManager.FindByEmailAsync(userCreateDto.Email);
            if (existingUser != null)
                throw new Exception("A user with this email already exists.");

            // Check if phone number already exists
            var existingPhoneUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == userCreateDto.PhoneNumber);
            if (existingPhoneUser != null)
                throw new Exception("A user with this phone number already exists.");

            // Check if the role exists
            var roleExists = await _roleManager.RoleExistsAsync(userCreateDto.Role);
            if (!roleExists)
                throw new Exception($"This Role does not exist.");

            // 1️ Create user in AspNetUsers
            var appUser = new ApplicationUser
            {
                UserName = userCreateDto.Email,
                Email = userCreateDto.Email,
                PhoneNumber = userCreateDto.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(appUser, userCreateDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {errors}");
            }

            await _userManager.AddToRoleAsync(appUser, userCreateDto.Role);

            // 2️ Create User entity and link with ApplicationUser
            var user = _mapper.Map<User>(userCreateDto);

            user.ApplicationUserId = appUser.Id;

            var folderPath = Path.Combine("Users", userCreateDto.Role); // Users/Admin, Users/Employee...
            user.ProfileImageFileName = _fileService.SaveFile(userCreateDto.Image, folderPath);

            try
            {
                await _userRepository.AddAsync(user);
                // 3️ Send welcome email
                var htmlBody = $@"
                    <h2>Welcome to Delivery Tracking System</h2>
                    <p>Dear <strong>{userCreateDto.FullName}</strong>,</p>
                    <p>Your account has been successfully created as a <strong>{userCreateDto.Role}</strong>.</p>
                    <p>You can now log in using your email: <strong>{userCreateDto.Email}</strong></p>
                    <br/>
                    <p>Thank you,<br/>Delivery Tracking System Team</p>
                    ";

                await _emailService.SendEmailAsync(userCreateDto.Email, "Welcome to Delivery Tracking System", htmlBody);

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

        public async Task UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new Exception("User not found!");
            _mapper.Map(dto, user);
            await _userRepository.UpdateAsync(id, user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new Exception("User not found!");
            await _userRepository.DeleteAsync(id);
        }

    }
}
