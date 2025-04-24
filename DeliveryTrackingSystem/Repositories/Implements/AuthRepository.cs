using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Models.Dtos.Auth;
using DeliveryTrackingSystem.Repositories.Interfaces;

namespace DeliveryTrackingSystem.Repositories.Implements
{
    public class AuthRepository : IAuthRepository
    {
        public Task AssignRoleAsync(ApplicationUser user, string role)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> CreateApplicationUserAsync(RequestRegisterDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
