using DeliveryTrackingSystem.Data;
using Microsoft.AspNetCore.Identity;

namespace DeliveryTrackingSystem.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string roleName);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> IsEmailValid(string email);
        Task<bool> IsPhoneNumberValid(string phoneNumber);
        Task<bool> PhoneExistsAsync(string phoneNumber);
        Task CreateRoleAsync(string roleName);
    }
}
