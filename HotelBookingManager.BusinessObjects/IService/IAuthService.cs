using HotelBookingManager.BusinessObjects.DTO;

namespace HotelBookingManager.BusinessObjects.Interfaces
{
    public class RegisterTempModel
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = "";
        public string? Gender { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? RoleId { get; set; }
    }


    public interface IAuthService
    {
        Task<UserDto?> ValidateUserAsync(string login, string password);
        Task<bool> IsUserNameTakenAsync(string userName);
        Task<bool> IsEmailTakenAsync(string email);

        Task StartRegisterOtpAsync(string email, RegisterTempModel data);
        Task<bool> VerifyRegisterOtpAsync(string email, string otp);
        Task<UserDto?> GetByEmailAsync(string email);

        Task StartForgotPasswordAsync(string email);
        Task<bool> VerifyResetOtpAsync(string email, string otp);
        Task ResetPasswordAsync(string email, string newPassword);
    }
}
