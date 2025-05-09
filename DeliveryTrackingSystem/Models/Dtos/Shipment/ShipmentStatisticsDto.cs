using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Dtos.Shipment
{
    public class ShipmentStatisticsDto
    {
        public int TotalShipments { get; set; }
        public decimal AverageCost { get; set; }
        public Dictionary<ShipmentStatus, int> StatusBreakdown { get; set; }
    }
}
