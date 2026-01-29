using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HotelBookingManager.Presentation.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IHotelService _hotelService;
        private readonly IRoomTypeService _roomTypeService;

        public RoomsController(
            IRoomService roomService,
            IHotelService hotelService,
            IRoomTypeService roomTypeService)
        {
            _roomService = roomService;
            _hotelService = hotelService;
            _roomTypeService = roomTypeService;
        }

        // GET: /Rooms
        public async Task<IActionResult> Index(string status = "active")
        {
            var rooms = await _roomService.GetByStatusAsync(status); // IEnumerable<RoomDto>
            ViewBag.Status = status;
            return View(rooms);
        }

        // GET: /Rooms/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new RoomDto());
        }

        // POST: /Rooms/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDto room)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(room);
            }

            await _roomService.CreateAsync(room);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Rooms/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetByIdAsync(id); // RoomDto?
            if (room == null) return NotFound();

            await LoadDropdowns();
            return View(room);
        }

        // POST: /Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomDto room)
        {
            if (id != room.RoomId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(room);
            }

            var ok = await _roomService.UpdateAsync(room);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomService.GetByIdAsync(id); // RoomDto?
            if (room == null) return NotFound();
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roomService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ByHotel(int hotelId, bool onlyAvailable = true)
        {
            var rooms = await _roomService.GetByHotelAsync(hotelId, onlyAvailable); // IEnumerable<RoomDto>
            ViewBag.HotelId = hotelId;
            ViewBag.OnlyAvailable = onlyAvailable;

            return View(rooms);
        }

        private async Task LoadDropdowns()
        {
            var hotels = await _hotelService.GetAllAsync();
            ViewBag.HotelId = new SelectList(hotels, "HotelId", "Name");

            var roomTypes = await _roomTypeService.GetAllAsync();
            ViewBag.RoomTypeId = new SelectList(roomTypes, "RoomTypeId", "Name");
        }
    }
}
