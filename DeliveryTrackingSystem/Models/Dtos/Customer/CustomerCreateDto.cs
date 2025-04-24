using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Dtos.Customer
{
    public class CustomerCreateDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(100)]
        [DefaultValue("Customer Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100)]
        [DefaultValue("customer@email.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(20)]
        [DefaultValue("01234567890")]
        public string PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
