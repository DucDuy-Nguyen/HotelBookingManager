using HotelBooking.BusinessObjects.Interfaces;
using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.Interfaces;
using HotelBookingManager.DataAccess.Models;
using HotelBookingManager.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;


namespace HotelBookingManager.BusinessObjects.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;

        public AuthService(IAuthRepository authRepository,
                           IEmailSender emailSender,
                           IMemoryCache cache)
        {
            _authRepository = authRepository;
            _emailSender = emailSender;
            _cache = cache;
        }


        // ========== LOGIN ==========

        private static UserDto ToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                HotelId = user.HotelId      // ĐẢM BẢO GÁN DÒNG NÀY

            };
        }


        // LOGIN
        public async Task<UserDto?> ValidateUserAsync(string login, string password)
        {
            var user = await _authRepository.GetUserByLoginAsync(login, password);
            if (user == null) return null;
            return ToUserDto(user);
        }

        public Task<bool> IsUserNameTakenAsync(string userName)
            => _authRepository.IsUserNameTakenAsync(userName);

        public Task<bool> IsEmailTakenAsync(string email)
            => _authRepository.IsEmailTakenAsync(email);

        // ========== OTP ĐĂNG KÝ (CHƯA TẠO USER) ==========

        private string BuildRegisterOtpKey(string email)
            => $"otp:register:{email.ToLower()}";

        private string BuildRegisterDataKey(string email)
            => $"register:data:{email.ToLower()}";

        private string GenerateOtp()
            => new Random().Next(100000, 999999).ToString();

        // Lưu thông tin đăng ký tạm + gửi OTP
        public async Task StartRegisterOtpAsync(string email, RegisterTempModel data)
        {
            var otp = GenerateOtp();
            var otpKey = BuildRegisterOtpKey(email);
            var dataKey = BuildRegisterDataKey(email);

            _cache.Set(otpKey, otp, TimeSpan.FromMinutes(5));
            _cache.Set(dataKey, data, TimeSpan.FromMinutes(5));

            await _emailSender.SendAsync(
                email,
                "Mã xác thực đăng ký tài khoản HotelBooking",
                $"Mã OTP của bạn là: {otp}. Mã có hiệu lực trong 5 phút.");
        }

        // OTP đúng → tạo user trong DB, trả về true/false
        public async Task<bool> VerifyRegisterOtpAsync(string email, string otp)
        {
            var otpKey = BuildRegisterOtpKey(email);
            var dataKey = BuildRegisterDataKey(email);

            if (!_cache.TryGetValue<string>(otpKey, out var storedOtp))
                return false;
            if (storedOtp != otp)
                return false;
            if (!_cache.TryGetValue<RegisterTempModel>(dataKey, out var data))
                return false;

            // Tạo user trong DB chỉ khi OTP đúng
            var user = new User
            {
                FullName = data.FullName,
                Email = data.Email,
                PhoneNumber = data.PhoneNumber,
                PasswordHash = data.Password, // demo
                Gender = data.Gender,
                IdentityNumber = data.IdentityNumber,
                DateOfBirth = data.DateOfBirth,
                RoleId = data.RoleId,
                CreatedAt = DateTime.Now,
                IsActive = true
            };


            await _authRepository.AddUserAsync(user);

            _cache.Remove(otpKey);
            _cache.Remove(dataKey);

            return true;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null) return null;
            return ToUserDto(user);
        }


        // ========== QUÊN MẬT KHẨU (giữ như cũ, dùng cache) ==========

        private string BuildResetOtpKey(string email)
            => $"otp:reset:{email.ToLower()}";

        public async Task StartForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null) return;

            var otp = GenerateOtp();
            var key = BuildResetOtpKey(email);

            _cache.Set(key, otp, TimeSpan.FromMinutes(5));

            await _emailSender.SendAsync(
                email,
                "Đặt lại mật khẩu",
                $"Mã OTP đặt lại mật khẩu của bạn là: {otp}. Mã có hiệu lực trong 5 phút.");
        }

        public Task<bool> VerifyResetOtpAsync(string email, string otp)
        {
            var key = BuildResetOtpKey(email);

            if (!_cache.TryGetValue<string>(key, out var storedOtp))
                return Task.FromResult(false);

            if (storedOtp != otp)
                return Task.FromResult(false);

            _cache.Remove(key);
            return Task.FromResult(true);
        }

        public async Task ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null) return;

            user.PasswordHash = newPassword;   // demo
            await _authRepository.UpdateAsync(user);
        }
    }
}
