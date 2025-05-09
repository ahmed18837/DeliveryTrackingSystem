using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory
{
    public class StatusHistoryFilterDto
    {
        public int? ShipmentId { get; set; }
        public ShipmentStatus? OldStatus { get; set; }
        public ShipmentStatus? NewStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
