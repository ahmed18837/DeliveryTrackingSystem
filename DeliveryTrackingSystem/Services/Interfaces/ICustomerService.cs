using DeliveryTrackingSystem.Models.Dtos.Customer;
using DeliveryTrackingSystem.Models.Dtos.Shipment;
using DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(int id);
        Task CreateAsync(CustomerCreateDto dto);
        Task UpdateAsync(int id, CustomerUpdateDto dto);
        Task DeleteAsync(int id);

        Task UpdateEmailAsync(int customerId, string email);
        Task<IEnumerable<ShipmentDto>> GetCustomerShipmentsAsync(int customerId);
        Task<IEnumerable<ShipmentStatusHistoryDto>> GetShipmentStatusHistoryAsync(int shipmentId);
        Task<CustomerShipmentSummaryDto> GetCustomerShipmentSummaryAsync(int customerId);
        Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm);
        Task<IEnumerable<CustomerDto>> FilterCustomersByShipmentCountAsync(int minShipments, int maxShipments);
    }
}
