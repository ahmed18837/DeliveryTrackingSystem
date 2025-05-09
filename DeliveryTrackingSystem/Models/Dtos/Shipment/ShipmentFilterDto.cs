using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Dtos.Shipment
{
    public class ShipmentFilterDto
    {
        public ShipmentStatus? Status { get; set; }
        public int? CustomerId { get; set; }
        public int? DriverId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
