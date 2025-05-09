using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;

namespace DeliveryTrackingSystem.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<IEnumerable<Shipment>> GetCustomerShipmentsAsync(int customerId);
        Task<IEnumerable<ShipmentStatusHistory>> GetShipmentStatusHistoryAsync(int shipmentId);
        Task<(int TotalShipments, decimal TotalCost, Dictionary<ShipmentStatus, int> StatusBreakdown)> GetCustomerShipmentSummaryAsync(int customerId);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
        Task<IEnumerable<Customer>> FilterCustomersByShipmentCountAsync(int minShipments, int maxShipments);
    }
}