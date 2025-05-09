using DeliveryTrackingSystem.Helper;

namespace DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory
{
    public partial class ShipmentStatusHistoryCreateDto
    {
        public class StatusChangeStatisticsDto
        {
            public int TotalChanges { get; set; }
            public Dictionary<ShipmentStatus, int> StatusTransitionCounts { get; set; }
        }
    }
}
