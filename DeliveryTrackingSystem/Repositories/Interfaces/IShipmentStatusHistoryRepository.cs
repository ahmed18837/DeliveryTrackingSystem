using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;

namespace DeliveryTrackingSystem.Repositories.Interfaces
{
    public interface IShipmentStatusHistoryRepository : IGenericRepository<ShipmentStatusHistory>
    {
        Task<IEnumerable<ShipmentStatusHistory>> GetStatusHistoryByShipmentIdAsync(int shipmentId);
        Task<ShipmentStatusHistory> GetLatestStatusChangeAsync(int shipmentId);
        Task<IEnumerable<ShipmentStatusHistory>> FilterStatusHistoryAsync(StatusHistoryFilter filter);
        Task<(int TotalChanges, Dictionary<ShipmentStatus, int> StatusTransitionCounts)> GetStatusChangeStatisticsAsync(int? shipmentId, DateTime? startDate, DateTime? endDate);
    }

    public class StatusHistoryFilter
    {
        public int? ShipmentId { get; set; }
        public ShipmentStatus? OldStatus { get; set; }
        public ShipmentStatus? NewStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}