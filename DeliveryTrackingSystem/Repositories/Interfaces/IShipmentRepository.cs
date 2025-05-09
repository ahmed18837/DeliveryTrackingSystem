using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Dtos.Shipment;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;

namespace DeliveryTrackingSystem.Repositories.Interfaces
{
    public interface IShipmentRepository : IGenericRepository<Shipment>
    {
        Task<Shipment> GetByTrackingNumberAsync(string trackingNumber);
        Task<IEnumerable<Shipment>> FilterShipmentsAsync(ShipmentFilterDto filter);
        Task<(int TotalShipments, decimal AverageCost, Dictionary<ShipmentStatus, int> StatusBreakdown)> GetShipmentStatisticsAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<(string Destination, int ShipmentCount)>> GetTopDestinationsAsync(int topN);
    }
}