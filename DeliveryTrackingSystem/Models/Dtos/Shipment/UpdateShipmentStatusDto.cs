using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Dtos.Shipment
{
    public class UpdateShipmentStatusDto
    {
        public ShipmentStatus NewStatus { get; set; }
    }
}
