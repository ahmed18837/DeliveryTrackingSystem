using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeliveryTrackingSystem.Models.Dtos.Auth
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [DefaultValue("user@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [DefaultValue("User")]
        public string Role { get; set; }
    }
}
