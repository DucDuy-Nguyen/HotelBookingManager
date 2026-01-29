using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelBookingManager.Presentation.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: /Payments
        public async Task<IActionResult> Index()
        {
            var payments = await _paymentService.GetAllAsync();
            return View(payments); // model: IEnumerable<PaymentDto>
        }

        // GET: /Payments/Pay?bookingId=1
        [HttpGet]
        public async Task<IActionResult> Pay(int bookingId)
        {
            PaymentDto payment;
            try
            {
                payment = await _paymentService.EnsurePaymentForBookingAsync(bookingId);
            }
            catch
            {
                return NotFound();
            }

            ViewBag.PaymentId = payment.PaymentId;
            ViewBag.BookingInfo =
                $"{payment.CheckInDate:dd/MM/yyyy} - {payment.CheckOutDate:dd/MM/yyyy}";
            ViewBag.Amount = payment.Amount;

            return View();
        }

        // POST: /Payments/Pay
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int paymentId, string method)
        {
            try
            {
                await _paymentService.PayAsync(paymentId, method);
            }
            catch
            {
                return NotFound();
            }

            return RedirectToAction("PayResult", new { paymentId });
        }

        // GET: /Payments/PayResult?paymentId=2
        public async Task<IActionResult> PayResult(int paymentId)
        {
            var payment = await _paymentService.GetPayResultAsync(paymentId);
            if (payment == null)
                return NotFound("Không tìm thấy giao dịch");

            return View(payment); // model: PaymentDto
        }

        // GET: /Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _paymentService.GetByIdAsync(id.Value);
            if (payment == null) return NotFound();

            return View(payment); // model: PaymentDto
        }
    }
}
