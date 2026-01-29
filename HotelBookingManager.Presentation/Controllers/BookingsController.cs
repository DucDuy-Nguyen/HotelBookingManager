using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // GET: Bookings
        public async Task<IActionResult> Index(string status = null)
        {
            IEnumerable<BookingDto> bookings;

            if (string.IsNullOrEmpty(status))
            {
                bookings = await _bookingService.GetAllAsync();
            }
            else
            {
                bookings = await _bookingService.GetByStatusAsync(status);
            }

            ViewBag.Status = status;
            return View(bookings);
        }


        // POST: Bookings/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            await _bookingService.ChangeStatusAsync(id, newStatus);
            return RedirectToAction(nameof(Index), new { status = newStatus });
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create(int? hotelId)
        {
            var booking = new BookingDto();
            if (hotelId.HasValue) booking.HotelId = hotelId.Value;

            await LoadDropdownsAsync(hotelId, null);
            return View(booking);
        }

        private int? GetCurrentUserId()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return null;

            var userIdStr = User.FindFirst("UserId")?.Value
                            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdStr, out var id) ? id : (int?)null;
        }
        // POST: Bookings/Create  ⭐⭐⭐ CHỖ SỬA DUY NHẤT
        [HttpPost]
        public async Task<IActionResult> Create(BookingDto booking)
        {
            if (booking.CheckOutDate <= booking.CheckInDate)
            {
                ModelState.AddModelError(string.Empty, "Check-out phải sau Check-in.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(booking.HotelId, booking.RoomId);
                return View(booking);
            }
            var userId = GetCurrentUserId();

            booking.UserId = userId.Value;

            var bookingId = await _bookingService.CreateAsync(booking);

            return RedirectToAction(
                "Pay",
                "Payments",
                new { bookingId }
            );
        }


        private async Task LoadDropdownsAsync(int? selectedHotelId = null, int? selectedRoomId = null)
        {
            var hotels = await _hotelService.GetAllAsync();
            ViewBag.HotelId = new SelectList(hotels, "HotelId", "Name", selectedHotelId);

            IEnumerable<RoomDto> rooms;

            if (selectedHotelId.HasValue && selectedHotelId.Value > 0)
            {
                rooms = await _roomService.GetByHotelAsync(selectedHotelId.Value, true);
            }
            else
            {
                rooms = new List<RoomDto>();
            }

            ViewBag.RoomId = new SelectList(rooms, "RoomId", "RoomNumber", selectedRoomId);
        }

    }
}
