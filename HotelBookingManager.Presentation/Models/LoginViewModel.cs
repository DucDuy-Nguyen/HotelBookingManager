namespace HotelBookingManager.Presentation.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
