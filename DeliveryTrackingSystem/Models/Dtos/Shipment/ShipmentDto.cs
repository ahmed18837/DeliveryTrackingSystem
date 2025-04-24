using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Dtos.Shipment
{
    public class ShipmentDto : ShipmentCreateDto
    {
        public int Id { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
