using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // 📜 LỊCH SỬ THANH TOÁN
        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var payments = await _paymentService.GetByUserAsync(userId);
            return View(payments);
        }

        // 🔥 ĐẶT PHÒNG → PAY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickPay(int roomId, int hotelId, int days = 1)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            int bookingId = await _bookingService.QuickCreateAsync(
                roomId, hotelId, days, userId
            );

            return RedirectToAction("Pay", new { bookingId });
        }

        // 💳 TRANG PAY
        [HttpGet]
        public async Task<IActionResult> Pay(int bookingId)
        {
            // tạo payment Pending nếu chưa có
            var payment = await _paymentService.EnsurePaymentForBookingAsync(
                bookingId, "Cash" // mặc định, user sẽ chọn lại
            );

            if (payment == null)
                return NotFound();

            return View(payment); // ⚠️ TRUYỀN MODEL
        }



        // ✅ XÁC NHẬN THANH TOÁN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int paymentId, string method)
        {
            await _paymentService.PayAsync(paymentId, method);
            return RedirectToAction("PayResult", new { paymentId });
        }


        public async Task<IActionResult> PayResult(int paymentId)
        {
            var payment = await _paymentService.GetPayResultAsync(paymentId);
            return View(payment);
        }

        public async Task<IActionResult> Details(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            return View(payment);
        }
    }
}
