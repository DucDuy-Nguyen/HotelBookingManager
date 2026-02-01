using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace HotelBookingManager.Presentation.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IBookingService _bookingService;

        public PaymentsController(
            IPaymentService paymentService,
            IBookingService bookingService)
        {
            _paymentService = paymentService;
            _bookingService = bookingService;
        }

        private int GetCurrentRoleId()
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            return string.IsNullOrEmpty(roleClaim) ? 3 : int.Parse(roleClaim);
        }

        private int? GetCurrentHotelId()
        {
            var hotelClaim = User.FindFirst("HotelId")?.Value;
            return int.TryParse(hotelClaim, out var hId) && hId > 0 ? hId : (int?)null;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst("UserId")?.Value ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdClaim, out var id) && id > 0 ? id : (int?)null;
        }

        // =========================
        // 📜 LỊCH SỬ THANH TOÁN
        // =========================
        public async Task<IActionResult> Index()
        {
            var roleId = GetCurrentRoleId();
            var userId = GetCurrentUserId();
            var hotelId = GetCurrentHotelId();

            IEnumerable<PaymentDto> payments;

            if (roleId == 1)
            {
                // Admin: xem tất cả
                payments = await _paymentService.GetAllAsync();
            }
            else if (roleId == 2)
            {
                // Staff: chỉ các payment thuộc hotel mình
                if (!hotelId.HasValue)
                {
                    payments = Enumerable.Empty<PaymentDto>();
                }
                else
                {
                    var all = await _paymentService.GetAllAsync();
                    payments = all.Where(p => p.HotelId == hotelId.Value);
                }
            }
            else
            {
                // Customer: chỉ của chính mình
                if (!userId.HasValue)
                {
                    payments = Enumerable.Empty<PaymentDto>();
                }
                else
                {
                    payments = await _paymentService.GetByUserAsync(userId.Value);
                }
            }

            return View(payments);
        }

        // =========================
        // 🔥 ĐẶT PHÒNG → QUICK PAY
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickPay(int roomId, int hotelId, int days = 1)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            int bookingId = await _bookingService.QuickCreateAsync(
                roomId, hotelId, days, userId.Value
            );

            return RedirectToAction("Pay", new { bookingId });
        }

        // =========================
        // 💳 TRANG PAY (GET)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Pay(int bookingId)
        {
            // tạo payment Pending nếu chưa có
            var payment = await _paymentService.EnsurePaymentForBookingAsync(
                bookingId, "Cash"
            );

            if (payment == null)
                return NotFound();

            // (tuỳ bạn) có thể kiểm tra thêm quyền tại đây

            return View(payment);
        }

        // =========================
        // ✅ XÁC NHẬN THANH TOÁN (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int paymentId, string method)
        {
            // (tuỳ chọn) có thể kiểm tra quyền giống Index:
            // staff chỉ được thu tiền payment của hotel mình.

            await _paymentService.PayAsync(paymentId, method);
            return RedirectToAction("PayResult", new { paymentId });
        }

        // =========================
        // 📄 KẾT QUẢ THANH TOÁN
        // =========================
        public async Task<IActionResult> PayResult(int paymentId)
        {
            var payment = await _paymentService.GetPayResultAsync(paymentId);
            if (payment == null) return NotFound();

            return View(payment);
        }

        // =========================
        // 🔍 CHI TIẾT THANH TOÁN
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();

            // (tuỳ chọn) kiểm tra:
            // - Admin: ok
            // - Staff: chỉ payment của hotel mình
            // - Customer: chỉ payment của chính mình

            return View(payment);
        }
    }
}
