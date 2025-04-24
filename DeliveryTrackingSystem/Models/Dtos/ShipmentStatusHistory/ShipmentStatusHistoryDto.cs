namespace DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory
{
    public class ShipmentStatusHistoryDto : ShipmentStatusHistoryCreateDto
    {
        public int Id { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
