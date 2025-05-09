using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;

namespace DeliveryTrackingSystem.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }
}
