using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;
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

        private int GetCurrentRoleId()
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            return string.IsNullOrEmpty(roleClaim) ? 3 : int.Parse(roleClaim);
        }

        private int? GetCurrentHotelId()
        {
            var hotelClaim = User.FindFirst("HotelId")?.Value;
            if (int.TryParse(hotelClaim, out var hId) && hId > 0)
                return hId;
            return null;
        }

        private bool CanManageRoom(RoomDto room)
        {
            var roleId = GetCurrentRoleId();
            if (roleId == 1) return true; // Admin

            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                return userHotelId.HasValue && userHotelId.Value == room.HotelId;
            }

            return false;
        }

        // =======================
        // GET: /Rooms
        // =======================
        public async Task<IActionResult> Index(string status = "active")
        {
            var roleId = GetCurrentRoleId();

            var rooms = await _roomService.GetByStatusAsync(status); // IEnumerable<RoomDto>

            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                if (userHotelId.HasValue)
                {
                    rooms = rooms.Where(r => r.HotelId == userHotelId.Value);
                }
                else
                {
                    rooms = Enumerable.Empty<RoomDto>();
                }
            }
            // role 1: thấy hết, role 3: cũng thấy hết (view ẩn nút sửa/xóa)

            ViewBag.Status = status;
            return View(rooms);
        }

        // =======================
        // GET: /Rooms/Create
        // =======================
        // GET: /Rooms/Create
        public async Task<IActionResult> Create()
        {
            var roleId = GetCurrentRoleId();
            if (roleId != 1 && roleId != 2)
                return Forbid();

            var userHotelId = GetCurrentHotelId();
            var hotels = await _hotelService.GetAllAsync();

            if (roleId == 2)
            {
                if (!userHotelId.HasValue)
                    return Forbid();
                hotels = hotels.Where(h => h.HotelId == userHotelId.Value);
            }

            ViewBag.HotelId = new SelectList(
                hotels,
                "HotelId",
                "Name",
                roleId == 2 ? userHotelId : null
            );

            var roomTypes = await _roomTypeService.GetAllAsync();
            ViewBag.RoomTypeId = new SelectList(roomTypes, "RoomTypeId", "Name");

            // ✨ THÊM: Lấy tất cả phòng, nhóm theo hotel
            using (var writer = new System.IO.StringWriter())
            {
                var allRooms = await _roomService.GetAllAsync();
                var allRoomsByHotel = new Dictionary<int, List<object>>();

                foreach (var room in allRooms)
                {
                    if (!allRoomsByHotel.ContainsKey(room.HotelId))
                        allRoomsByHotel[room.HotelId] = new List<object>();

                    allRoomsByHotel[room.HotelId].Add(new
                    {
                        room.RoomId,
                        room.RoomNumber,
                        room.HotelId
                    });
                }

                // ✨ Pass dữ liệu cho view dưới dạng JSON
                var json = System.Text.Json.JsonSerializer.Serialize(allRoomsByHotel);
                ViewBag.AllRoomsByHotel = json;
            }

            var model = new RoomDto();
            if (roleId == 2 && userHotelId.HasValue)
                model.HotelId = userHotelId.Value;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDto room)
        {
            var roleId = GetCurrentRoleId();
            if (roleId != 1 && roleId != 2)
                return Forbid();

            // Staff chỉ được tạo phòng cho hotel của mình
            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                if (!userHotelId.HasValue || userHotelId.Value != room.HotelId)
                    return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsForEditOrCreate(roleId, room.HotelId, room.RoomTypeId);
                return View(room);
            }

            // ✨ THÊM: Validate ở server (backup khi user bypass JS)
            var existingRooms = await _roomService.GetByHotelAsync(room.HotelId, false);
            if (existingRooms.Any(r => r.RoomNumber == room.RoomNumber))
            {
                ModelState.AddModelError("RoomNumber",
                    $"❌ Phòng số {room.RoomNumber} đã tồn tại!");
                await LoadDropdownsForEditOrCreate(roleId, room.HotelId, room.RoomTypeId);
                return View(room);
            }

            await _roomService.CreateAsync(room);
            return RedirectToAction(nameof(Index));
        }


        // =======================
        // GET: /Rooms/Edit/5
        // =======================
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetByIdAsync(id); // RoomDto?
            if (room == null) return NotFound();

            if (!CanManageRoom(room)) return Forbid();

            await LoadDropdownsForEditOrCreate(GetCurrentRoleId(), room.HotelId, room.RoomTypeId);
            return View(room);
        }

        // =======================
        // POST: /Rooms/Edit/5
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomDto room)
        {
            if (id != room.RoomId) return BadRequest();

            if (!CanManageRoom(room)) return Forbid();

            if (!ModelState.IsValid)
            {
                await LoadDropdownsForEditOrCreate(GetCurrentRoleId(), room.HotelId, room.RoomTypeId);
                return View(room);
            }

            var ok = await _roomService.UpdateAsync(room);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =======================
        // GET: Rooms/Delete/5
        // =======================
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomService.GetByIdAsync(id); // RoomDto?
            if (room == null) return NotFound();

            if (!CanManageRoom(room)) return Forbid();

            return View(room);
        }

        // =======================
        // POST: Rooms/Delete/5
        // =======================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            if (!CanManageRoom(room)) return Forbid();

            await _roomService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // =======================
        // GET: /Rooms/ByHotel
        // =======================
        [HttpGet]
        public async Task<IActionResult> ByHotel(int hotelId, bool onlyAvailable = true)
        {
            var roleId = GetCurrentRoleId();

            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                if (!userHotelId.HasValue || userHotelId.Value != hotelId)
                    return Forbid();
            }

            var rooms = await _roomService.GetByHotelAsync(hotelId, onlyAvailable); // IEnumerable<RoomDto>
            ViewBag.HotelId = hotelId;
            ViewBag.OnlyAvailable = onlyAvailable;

            return View(rooms);
        }

        // =======================
        // Dropdown helper
        // =======================
        private async Task LoadDropdownsForEditOrCreate(int roleId, int? selectedHotelId, int? selectedRoomTypeId)
        {
            var hotels = await _hotelService.GetAllAsync();

            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                if (userHotelId.HasValue)
                    hotels = hotels.Where(h => h.HotelId == userHotelId.Value);
            }

            ViewBag.HotelId = new SelectList(hotels, "HotelId", "Name", selectedHotelId);

            var roomTypes = await _roomTypeService.GetAllAsync();
            ViewBag.RoomTypeId = new SelectList(roomTypes, "RoomTypeId", "Name", selectedRoomTypeId);
        }
    }
}
