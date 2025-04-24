using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string? Address { get; set; }

        public ICollection<Shipment> Shipments { get; set; }
    }
}
