using System.ComponentModel.DataAnnotations;

namespace HotelBookingManager.Presentation.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Gender { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? ErrorMessage { get; set; }
    }

}
