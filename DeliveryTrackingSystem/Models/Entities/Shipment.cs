using DeliveryTrackingSystem.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryTrackingSystem.Models.Entities
{
    public class Shipment
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string TrackingNumber { get; set; }

        public string Origin { get; set; }
        public string Destination { get; set; }

        public DateTime CreatedAt { get; set; }

        public ShipmentStatus Status { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal WeightKg { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ShippingCost { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int? DriverId { get; set; } // من جدول User
        public User? Driver { get; set; }

        public ICollection<ShipmentStatusHistory> StatusHistories { get; set; }
    }
}
