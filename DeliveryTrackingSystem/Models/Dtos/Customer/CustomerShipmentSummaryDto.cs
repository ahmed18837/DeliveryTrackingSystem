using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Dtos.Customer
{
    public class CustomerShipmentSummaryDto
    {
        public int TotalShipments { get; set; }
        public decimal TotalCost { get; set; }
        public Dictionary<ShipmentStatus, int> StatusBreakdown { get; set; }
    }
}