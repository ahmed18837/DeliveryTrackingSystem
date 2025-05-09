using DeliveryTrackingSystem.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory
{
    public partial class ShipmentStatusHistoryCreateDto
    {
        [Required]
        [DefaultValue(ShipmentStatus.Pending)]
        public ShipmentStatus OldStatus { get; set; }

        [Required]
        [DefaultValue(ShipmentStatus.InTransit)]
        public ShipmentStatus NewStatus { get; set; }

        [Required]
        [DefaultValue(1)]
        public int ShipmentId { get; set; }
    }
}
