using DeliveryTrackingSystem.Models.Dtos.Auth;
using DeliveryTrackingSystem.Models.Dtos.User;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task AdminRegisterAsync(RequestRegisterDto request);
        Task EmployeeRegisterAsync(RequestRegisterDto request);
        Task DriverRegisterAsync(RequestRegisterDto request);
        Task<ResponseDto> LoginAsync(LoginDto request);
        Task<string> AssignRoleAsync(string email, string roleName);
        Task<string> ForgetPasswordAsync(string email);
        //Task<string> ResetPasswordAsync(ResetPasswordDto model);
        Task<string> Send2FACodeAsync(string email);
        Task<string> Verify2FACodeAsync(Verify2FACodeDto model);
        public Task<string> UnlockUserAsync(string email);
        Task<string> ChangePasswordAsync(ChangePasswordDto model);
        Task<IEnumerable<string>> GetAllRolesAsync();
        Task<IEnumerable<string>> GetRolesByEmailAsync(string email);
        Task DeleteRoleAsync(string roleName);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName);
        public Task UpdateRoleAsync(UpdateRoleDto model);
        Task<string> Resend2FACodeAsync(string email);
        Task AddRoleAsync(string roleName);
        Task RemoveUserFromRoleAsync(string email, string role);
        Task LogoutAsync(string userEmail);

    }
}
