using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Entities
{
    public class ShipmentStatusHistory
    {
        public int Id { get; set; }
        public ShipmentStatus OldStatus { get; set; }
        public ShipmentStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }

        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
    }
}