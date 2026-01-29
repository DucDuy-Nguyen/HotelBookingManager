using System.ComponentModel.DataAnnotations;

namespace HotelBookingManager.Presentation.Models
{
    public class VerifyOtpViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mã OTP")]
        public string OtpCode { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }
}
