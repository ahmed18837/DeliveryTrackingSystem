using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Dtos.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(50, ErrorMessage = "Full Name cannot exceed 50 characters.")]
        [DefaultValue("FullName")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01\d{9}$", ErrorMessage = "Phone number must start with '01' and be exactly 11 digits.")]
        [MaxLength(11, ErrorMessage = "Phone number must be exactly 11 digits.")]
        [MinLength(11, ErrorMessage = "Phone number must be exactly 11 digits.")]
        [DefaultValue("01012345678")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        [DefaultValue("user@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DefaultValue("Password123")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [MaxLength(20, ErrorMessage = "Role cannot exceed 20 characters.")]
        [DefaultValue("Employee")]
        public string Role { get; set; } // Admin, Employee, Driver, etc.

        [Required(ErrorMessage = "Image is required.")]
        public IFormFile Image { get; set; }
    }
}
