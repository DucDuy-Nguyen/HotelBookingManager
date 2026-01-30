using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HotelBookingManager.Presentation.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IHotelService _hotelService;
        private readonly IRoomService _roomService;

        public BookingsController(
            IBookingService bookingService,
            IHotelService hotelService,
            IRoomService roomService)
        {
            _bookingService = bookingService;
            _hotelService = hotelService;
            _roomService = roomService;
        }

        // ===============================
        // 📜 LỊCH SỬ ĐẶT PHÒNG
        // ===============================
        public async Task<IActionResult> Index(string status = null)
        {
            var bookings = string.IsNullOrEmpty(status)
                ? await _bookingService.GetAllAsync()
                : await _bookingService.GetByStatusAsync(status);

            ViewBag.Status = status;
            return View(bookings);
        }

        // ===============================
        // 🆕 CREATE (GET)
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Create(int? roomId, int? hotelId)
        {
            bool lockSelection = roomId.HasValue && hotelId.HasValue;
            ViewBag.LockSelection = lockSelection;

            var model = new BookingDto
            {
                RoomId = roomId ?? 0,
                HotelId = hotelId ?? 0
            };

            await LoadDropdownsAsync(model.HotelId, model.RoomId);

            return View(model);
        }

        // ===============================
        // 🆕 CREATE (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingDto booking)
        {
            if (booking.CheckOutDate <= booking.CheckInDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Ngày Check-out phải sau ngày Check-in."
                );
            }

            if (!ModelState.IsValid)
            {
                ViewBag.LockSelection = booking.RoomId > 0 && booking.HotelId > 0;
                await LoadDropdownsAsync(booking.HotelId, booking.RoomId);
                return View(booking);
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            booking.UserId = userId.Value;

            int bookingId = await _bookingService.CreateAsync(booking);

            // 👉 sau khi tạo booking → sang thanh toán
            return RedirectToAction(
                "Pay",
                "Payments",
                new { bookingId }
            );
        }

        // ===============================
        // 🔁 LOAD DROPDOWNS
        // ===============================
        private async Task LoadDropdownsAsync(int? selectedHotelId, int? selectedRoomId)
        {
            var hotels = await _hotelService.GetAllAsync();
            ViewBag.HotelId = new SelectList(hotels, "HotelId", "Name", selectedHotelId);

            var rooms = selectedHotelId.HasValue && selectedHotelId > 0
                ? await _roomService.GetByHotelAsync(selectedHotelId.Value, true)
                : new List<RoomDto>();

            ViewBag.RoomId = new SelectList(rooms, "RoomId", "RoomNumber", selectedRoomId);
        }

        // ===============================
        // 👤 USER ID
        // ===============================
        private int? GetCurrentUserId()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return null;

            var userIdStr =
                User.FindFirstValue("UserId") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdStr, out int id) ? id : null;
        }
    }
}
