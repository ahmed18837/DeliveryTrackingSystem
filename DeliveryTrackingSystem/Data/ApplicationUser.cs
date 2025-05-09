using DeliveryTrackingSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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

        [MaxLength(8)]
        public string? ResetCode { get; set; }

        [MaxLength(8)]
        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorCodeExpiration { get; set; }
        public DateTime? TwoFactorSentAt { get; set; } // وقت إرسال الكود الأخير
        public int TwoFactorAttempts { get; set; } = 0; // عدد المحاولات خلال الساعة
        public DateTime? LastTwoFactorAttempt { get; set; } // آخر وقت للمحاولة

        // محاولات الفشل وقفل الحساب
        public int FailedTwoFactorAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; } // متى ينتهي القفل؟

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
