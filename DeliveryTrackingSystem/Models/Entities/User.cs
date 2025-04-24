using DeliveryTrackingSystem.Data;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } // Driver - Employee - Admin

        public string? ProfileImageFileName { get; set; }

        public string ApplicationUserId { get; set; } // ربط مع AspNetUsers
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Shipment>? AssignedShipments { get; set; }
    }
}
