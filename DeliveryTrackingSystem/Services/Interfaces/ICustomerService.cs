using DeliveryTrackingSystem.Models.Dtos.Customer;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(int id);
        Task CreateAsync(CustomerCreateDto dto);
        Task UpdateAsync(int id, CustomerUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
