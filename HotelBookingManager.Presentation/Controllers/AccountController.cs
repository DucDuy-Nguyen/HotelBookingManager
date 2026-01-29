using HotelBooking.BusinessObjects.Interfaces;
using HotelBookingManager.BusinessObjects.Interfaces;
using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.Presentation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace HotelBookingManager.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;

        public AccountController(IAuthService authService, IEmailSender emailSender)
        {
            _authService = authService;
            _emailSender = emailSender; 

        }

        [HttpGet]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.ValidateUserAsync(model.Email, model.Password);
            if (user == null)
            {
                model.ErrorMessage = "Sai email hoặc mật khẩu, hoặc tài khoản đã bị khóa.";
                return View(model);
            }

            await SignInUser(user);


            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                model.ErrorMessage = "Mật khẩu nhập lại không khớp.";
                return View(model);
            }

            if (await _authService.IsEmailTakenAsync(model.Email))
            {
                model.ErrorMessage = "Email đã được sử dụng.";
                return View(model);
            }

            var temp = new RegisterTempModel
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                Gender = model.Gender,
                IdentityNumber = model.IdentityNumber,
                DateOfBirth = model.DateOfBirth,
                RoleId = 3 // Customer
            };

            await _authService.StartRegisterOtpAsync(model.Email, temp);

            TempData["RegisterEmail"] = model.Email;
            return RedirectToAction("VerifyRegisterOtp");
        }



        [HttpGet]
        public IActionResult VerifyRegisterOtp()
        {
            var email = TempData["RegisterEmail"] as string;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Register");

            return View(new VerifyOtpViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyRegisterOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await _authService.VerifyRegisterOtpAsync(model.Email, model.OtpCode);
            if (!ok)
            {
                model.ErrorMessage = "Mã OTP không đúng hoặc đã hết hạn.";
                return View(model);
            }

            var user = await _authService.GetByEmailAsync(model.Email);
            if (user != null)
            {
                await SignInUser(user);
            }

            return RedirectToAction("Index", "Home");
        }







        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(UserDto user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, (user.RoleId ?? 3).ToString())
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }




        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _authService.StartForgotPasswordAsync(model.Email);

            TempData["ResetEmail"] = model.Email;
            return RedirectToAction("VerifyResetOtp");
        }

        [HttpGet]
        public IActionResult VerifyResetOtp()
        {
            var email = TempData["ResetEmail"] as string;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("ForgotPassword");

            return View(new VerifyResetOtpViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyResetOtp(VerifyResetOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await _authService.VerifyResetOtpAsync(model.Email, model.OtpCode);
            if (!ok)
            {
                model.ErrorMessage = "Mã OTP không đúng hoặc đã hết hạn.";
                return View(model);
            }

            TempData["ResetEmail"] = model.Email;
            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var email = TempData["ResetEmail"] as string;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("ForgotPassword");

            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                model.ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return View(model);
            }

            await _authService.ResetPasswordAsync(model.Email, model.NewPassword);
            return RedirectToAction("Login");
        }



    }
}
