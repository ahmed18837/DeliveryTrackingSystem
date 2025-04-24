using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Models.Dtos.Auth;

namespace DeliveryTrackingSystem.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> CreateApplicationUserAsync(RequestRegisterDto dto);
        Task AssignRoleAsync(ApplicationUser user, string role);
    }
}
