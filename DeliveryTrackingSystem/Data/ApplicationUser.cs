using DeliveryTrackingSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace DeliveryTrackingSystem.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public string? LastLoginIp { get; set; }
        public string? LastLoginDevice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? DeactivationDate { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
