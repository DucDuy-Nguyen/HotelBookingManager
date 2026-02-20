using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IRoomImageService _roomImageService;
        private readonly IWebHostEnvironment _env;


        public RoomsController(
            IRoomService roomService,
            IHotelService hotelService,
            IRoomTypeService roomTypeService,
            IRoomImageService roomImageService,             IWebHostEnvironment env)
        {
            _roomService = roomService;
            _hotelService = hotelService;
            _roomTypeService = roomTypeService;
            _roomImageService = roomImageService;
            _env = env;
        }

        // =======================
        // Helpers
        // =======================
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
        public async Task<IActionResult> Recommendations()
        {
            var hotels = await _hotelService.GetAllAsync();
            ViewBag.Hotels = new SelectList(hotels, "HotelId", "Name");
            return View();
        }
        public async Task<IActionResult> Index(string status = "active", int pageNumber = 1)
        {
            var roleId = GetCurrentRoleId();
            var rooms = await _roomService.GetByStatusAsync(status);

            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                rooms = userHotelId.HasValue
                    ? rooms.Where(r => r.HotelId == userHotelId.Value).ToList()
                    : new List<RoomDto>();
            }

            // 🆕 ENRICH IMAGES - giữ nguyên code anh
            foreach (var room in rooms)
            {
                var roomImages = await _roomImageService.GetByRoomIdAsync(room.RoomId);
                if (roomImages.Any())
                {
                    room.FirstImageUrl = roomImages.FirstOrDefault(img => img.IsThumbnail)?.ImageUrl
                                       ?? roomImages.First().ImageUrl;
                    room.RoomImagesCount = roomImages.Count();
                    room.RoomImages = roomImages.ToList();
                }
            }

            // PHÂN TRANG
            int pageSize = 10; // Số phòng mỗi trang (có thể chỉnh 12, 15...)
            int totalRooms = rooms.Count();
            var pagedRooms = rooms.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Truyền ViewBag để view dùng phân trang
            ViewBag.Status = status;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRooms / pageSize);

            return View(pagedRooms); // Trả danh sách đã phân trang
        }



        // =======================
        // GET: /Rooms/Details/5
        // =======================
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            return View(room);
        }

        // =======================
        // GET: /Rooms/Create
        // =======================
        public async Task<IActionResult> Create()
        {
            var roleId = GetCurrentRoleId();
            if (roleId != 1 && roleId != 2)
                return Forbid();

            var userHotelId = GetCurrentHotelId();
            var hotels = await _hotelService.GetAllAsync();

            if (roleId == 2)
            {
                if (!userHotelId.HasValue) return Forbid();
                hotels = hotels.Where(h => h.HotelId == userHotelId.Value);
            }

            ViewBag.HotelId = new SelectList(
                hotels,
                "HotelId",
                "Name",
                roleId == 2 ? userHotelId : null
            );

            ViewBag.RoomTypeId = new SelectList(
                await _roomTypeService.GetAllAsync(),
                "RoomTypeId",
                "Name"
            );

            // dữ liệu check phòng trùng
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

            ViewBag.AllRoomsByHotel =
                System.Text.Json.JsonSerializer.Serialize(allRoomsByHotel);

            var vm = new RoomCreateEditViewModel
            {
                HotelId = roleId == 2 && userHotelId.HasValue ? userHotelId.Value : 0,
            };

            return View(vm);
        }

        // =======================
        // POST: /Rooms/Create
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomCreateEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsForEditOrCreate(
                    GetCurrentRoleId(),
                    vm.HotelId,
                    vm.RoomTypeId
                );
                return View(vm);
            }

            // 1️⃣ Tạo room trước
            var roomId = await _roomService.CreateAsync(new RoomCreateUpdateDto
            {
                HotelId = vm.HotelId,
                RoomTypeId = vm.RoomTypeId,
                RoomNumber = vm.RoomNumber,
                Floor = vm.Floor,
                CurrentPrice = vm.CurrentPrice
            });

            // ==================================================
            // 2️⃣ UPLOAD ẢNH – ĐOẠN BẠN HỎI NẰM Ở ĐÂY ⬇⬇⬇
            // ==================================================
            if (vm.Images != null && vm.Images.Any())
            {
                var uploadRoot = Path.Combine(
                    _env.WebRootPath,   // wwwroot (runtime)
                    "uploads",
                    "rooms"
                );

                if (!Directory.Exists(uploadRoot))
                {
                    Directory.CreateDirectory(uploadRoot);
                }

                var urls = new List<string>();

                foreach (var file in vm.Images)
                {
                    if (file.Length == 0) continue;

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadRoot, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    urls.Add("/uploads/rooms/" + fileName);
                }

                await _roomImageService.AddImagesAsync(roomId, urls);
            }
            // ==================================================

            return RedirectToAction("Details", new { id = roomId });
        }


        // =======================
        // GET: /Rooms/Edit/5
        // =======================
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();
            if (!CanManageRoom(room)) return Forbid();

            // Map sang ViewModel
            var vm = new RoomCreateEditViewModel
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomTypeId = room.RoomTypeId,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor ?? 0,
                CurrentPrice = room.CurrentPrice,
                Status = room.Status,
                IsActive = room.IsActive,
                HotelName = room.HotelName,
                RoomTypeName = room.RoomTypeName,
                RoomImages = (await _roomImageService.GetByRoomIdAsync(id)).ToList()
            };

            // Load dropdowns
            await LoadDropdownsForEditOrCreate(GetCurrentRoleId(), room.HotelId, room.RoomTypeId);

            // ✅ ViewBag cho JS validation (copy từ Create)
            var allRooms = await _roomService.GetAllAsync();
            var allRoomsByHotel = new Dictionary<int, List<object>>();
            foreach (var r in allRooms)
            {
                if (!allRoomsByHotel.ContainsKey(r.HotelId))
                    allRoomsByHotel[r.HotelId] = new List<object>();
                allRoomsByHotel[r.HotelId].Add(new { r.RoomId, r.RoomNumber, r.HotelId });
            }
            ViewBag.AllRoomsByHotel = System.Text.Json.JsonSerializer.Serialize(allRoomsByHotel);

            return View(vm);
        }




        // =======================
        // POST: /Rooms/Edit/5
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomCreateEditViewModel vm)
        {
            if (id != vm.RoomId) return BadRequest();

            // CHECK QUYỀN
            var tempDto = new RoomDto { RoomId = vm.RoomId ?? 0, HotelId = vm.HotelId };
            if (!CanManageRoom(tempDto)) return Forbid();

            if (!ModelState.IsValid)
            {
                await LoadDropdownsForEditOrCreate(GetCurrentRoleId(), vm.HotelId, vm.RoomTypeId);
                vm.RoomImages = (await _roomImageService.GetByRoomIdAsync(id)).ToList();

                // ✅ THÊM ViewBag cho JS validation
                var allRooms = await _roomService.GetAllAsync();
                var allRoomsByHotel = new Dictionary<int, List<object>>();
                foreach (var r in allRooms)
                {
                    if (!allRoomsByHotel.ContainsKey(r.HotelId))
                        allRoomsByHotel[r.HotelId] = new List<object>();
                    allRoomsByHotel[r.HotelId].Add(new { r.RoomId, r.RoomNumber, r.HotelId });
                }
                ViewBag.AllRoomsByHotel = System.Text.Json.JsonSerializer.Serialize(allRoomsByHotel);

                return View(vm);
            }

            // 1️⃣ UPDATE ROOM INFO
            var roomDto = new RoomDto
            {
                RoomId = vm.RoomId ?? 0,
                HotelId = vm.HotelId,
                RoomTypeId = vm.RoomTypeId,
                RoomNumber = vm.RoomNumber,
                Floor = vm.Floor,
                Status = vm.Status ?? "Available",
                CurrentPrice = vm.CurrentPrice,
                IsActive = vm.IsActive
            };

            var ok = await _roomService.UpdateAsync(roomDto);
            if (!ok) return NotFound();

            // 🔥 2️⃣ UPLOAD NEW IMAGES - COPY TỪ CREATE
            if (vm.Images != null && vm.Images.Any())
            {
                var uploadRoot = Path.Combine(_env.WebRootPath, "uploads", "rooms");
                if (!Directory.Exists(uploadRoot))
                    Directory.CreateDirectory(uploadRoot);

                var urls = new List<string>();
                foreach (var file in vm.Images)
                {
                    if (file.Length == 0) continue;

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadRoot, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    urls.Add("/uploads/rooms/" + fileName);
                }

                await _roomImageService.AddImagesAsync(id, urls);
            }

            TempData["Success"] = "Cập nhật phòng thành công!";
            return RedirectToAction(nameof(Index));
        }





        // =======================
        // GET: /Rooms/Delete/5
        // =======================
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            if (!CanManageRoom(room)) return Forbid();

            return View(room);
        }

        // =======================
        // POST: /Rooms/Delete/5
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

            // 🆕 SỬA: Lấy rooms + ENRICH ẢNH
            var rooms = await _roomService.GetByHotelAsync(hotelId, onlyAvailable);

            // 🆕 THÊM ENRICH IMAGES (giống Index)
            foreach (var room in rooms)
            {
                if (room.RoomImages == null || !room.RoomImages.Any())
                {
                    var roomImages = await _roomImageService.GetByRoomIdAsync(room.RoomId);
                    if (roomImages.Any())
                    {
                        room.FirstImageUrl = roomImages.FirstOrDefault(img => img.IsThumbnail)?.ImageUrl ?? roomImages.First().ImageUrl;
                        room.RoomImagesCount = roomImages.Count();
                        room.RoomImages = roomImages.ToList();
                    }
                }
            }

            ViewBag.HotelId = hotelId;
            ViewBag.OnlyAvailable = onlyAvailable;
            return View(rooms);
        }

        // 🔥 THÊM VÀO CUỐI CONTROLLER, TRƯỚC LoadDropdownsForEditOrCreate
        // =======================
        // DELETE IMAGE - AJAX
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var roomImage = await _roomImageService.GetByIdAsync(id);
                if (roomImage == null)
                    return Json(new { success = false, message = "Ảnh không tồn tại" });

                var room = await _roomService.GetByIdAsync(roomImage.RoomId);
                if (room == null || !CanManageRoom(room))
                    return Json(new { success = false, message = "Không có quyền" });

                // Xóa file
                var filePath = Path.Combine(_env.WebRootPath, roomImage.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                // Xóa DB
                await _roomImageService.DeleteAsync(id);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Lỗi server" });
            }
        }




        // =======================
        // Dropdown helper
        // =======================
        private async Task LoadDropdownsForEditOrCreate(
            int roleId,
            int? selectedHotelId,
            int? selectedRoomTypeId)
        {
            var hotels = await _hotelService.GetAllAsync();

            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                if (userHotelId.HasValue)
                    hotels = hotels.Where(h => h.HotelId == userHotelId.Value);
            }

            ViewBag.HotelId = new SelectList(
                hotels,
                "HotelId",
                "Name",
                selectedHotelId
            );

            ViewBag.RoomTypeId = new SelectList(
                await _roomTypeService.GetAllAsync(),
                "RoomTypeId",
                "Name",
                selectedRoomTypeId
            );
        }
    }
}
