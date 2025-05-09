using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Dtos.Auth
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        [DefaultValue("user@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Reset Code is required.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Reset Code must be 8 characters.")]
        [DefaultValue("12345678")]
        public string ResetCode { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(64, ErrorMessage = "Password cannot exceed 64 characters.")]
        [DefaultValue("SecurePass123")]
        public string NewPassword { get; set; }
    }
}
