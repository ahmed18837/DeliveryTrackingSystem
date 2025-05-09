using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Dtos.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        [DefaultValue("user@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(64, ErrorMessage = "Password cannot exceed 64 characters.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DefaultValue("SecurePass123")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(64, ErrorMessage = "Password cannot exceed 64 characters.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DefaultValue("SecurePass123")]
        public string NewPassword { get; set; }
    }
}
