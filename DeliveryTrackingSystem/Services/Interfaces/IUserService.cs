using DeliveryTrackingSystem.Models.Dtos.Auth;
using DeliveryTrackingSystem.Models.Dtos.User;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface IUserService
    {
        // CRUD
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task CreateAsync(RegisterDto userCreateDto);
        Task UpdateAsync(int id, UserUpdateDto dto);
        Task DeleteAsync(int id);
        Task<UserDto> GetByEmailAsync(string email);
    }
}
