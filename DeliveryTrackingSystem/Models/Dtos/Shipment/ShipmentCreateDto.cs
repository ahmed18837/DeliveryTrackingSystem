using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Dtos.Shipment
{
    public class ShipmentCreateDto
    {
        [Required(ErrorMessage = "Tracking Number is required.")]
        [MaxLength(50)]
        [DefaultValue("TRK123456789")]
        public string TrackingNumber { get; set; }

        [Required(ErrorMessage = "Origin is required.")]
        [DefaultValue("Cairo")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        [DefaultValue("Alexandria")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [DefaultValue(2.5)]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "Shipping cost is required.")]
        [DefaultValue(50)]
        public decimal ShippingCost { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        [DefaultValue(5)]
        public int CustomerId { get; set; }

        public int? DriverId { get; set; }
    }
}
