using System.Security.Claims;
using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingManager.Presentation.Controllers
{
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // ====== helper lấy RoleId & HotelId từ claims ======
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

        private bool CanManageHotel(int hotelId)
        {
            var roleId = GetCurrentRoleId();

            if (roleId == 1) return true; // Admin

            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                return userHotelId.HasValue && userHotelId.Value == hotelId;
            }

            return false;
        }

        // GET: /Hotels
        public async Task<IActionResult> Index(string? city, string? statusFilter)
        {
            statusFilter ??= "active";


            var roleId = GetCurrentRoleId();
            var hotels = await _hotelService.GetFilteredAsync(city, statusFilter); // IEnumerable<HotelDto>

            // Lọc theo hotel của staff
            if (roleId == 2)
            {
                var userHotelId = GetCurrentHotelId();
                if (userHotelId.HasValue)
                {
                    hotels = hotels.Where(h => h.HotelId == userHotelId.Value);
                }
                else
                {
                    hotels = Enumerable.Empty<HotelDto>();
                }
            }
           

            var model = new HotelSearchViewModel
            {
                City = city,
                StatusFilter = statusFilter,
                Hotels = hotels.Select(h => new HotelViewModel
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    Description = h.Description,
                    Address = h.Address,
                    City = h.City,
                    Country = h.Country,
                    Rating = (decimal?)h.Rating,
                    PhoneNumber = h.PhoneNumber,
                    Email = h.Email,
                    IsActive = h.IsActive,
                    ImageUrl = h.ImageUrl
                }).ToList()
            };

            return View(model);
        }

        // GET: /Hotels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id); // HotelDto?
            if (hotel == null) return NotFound();

            // Cho phép cả role 1,2,3 xem chi tiết nếu bạn muốn
            var model = new HotelViewModel
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                Rating = (decimal?)hotel.Rating,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                IsActive = hotel.IsActive,
                ImageUrl = hotel.ImageUrl
            };

            return View(model);
        }

        // GET: /Hotels/Create
        public IActionResult Create()
        {
            var roleId = GetCurrentRoleId();
            if (roleId != 1) return Forbid(); // chỉ Admin được tạo khách sạn mới

            return View();
        }

        // POST: /Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelViewModel model)
        {
            var roleId = GetCurrentRoleId();
            if (roleId != 1) return Forbid(); // chỉ Admin

            if (!ModelState.IsValid) return View(model);

            var hotelDto = new HotelDto
            {
                Name = model.Name,
                Description = model.Description,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                Rating = (double?)model.Rating,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                IsActive = model.IsActive,
                ImageUrl = model.ImageUrl
            };

            await _hotelService.CreateAsync(hotelDto);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Hotels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id); // HotelDto?
            if (hotel == null) return NotFound();

            if (!CanManageHotel(id)) return Forbid();

            var model = new HotelViewModel
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                Rating = (decimal?)hotel.Rating,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                IsActive = hotel.IsActive,
                ImageUrl = hotel.ImageUrl
            };

            return View(model);
        }

        // POST: /Hotels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelViewModel model)
        {
            if (id != model.HotelId) return BadRequest();
            if (!CanManageHotel(id)) return Forbid();

            if (!ModelState.IsValid) return View(model);

            var hotelDto = new HotelDto
            {
                HotelId = model.HotelId,
                Name = model.Name,
                Description = model.Description,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                Rating = (double?)model.Rating,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                IsActive = model.IsActive,
                ImageUrl = model.ImageUrl
            };

            var ok = await _hotelService.UpdateAsync(hotelDto);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Hotels/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id); // HotelDto?
            if (hotel == null) return NotFound();

            if (!CanManageHotel(id)) return Forbid();

            var model = new HotelViewModel
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                Rating = (decimal?)hotel.Rating,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                IsActive = hotel.IsActive,
                ImageUrl = hotel.ImageUrl
            };

            return View(model);
        }

        // POST: /Hotels/Delete/5 (soft delete: IsActive = false trong service)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!CanManageHotel(id)) return Forbid();

            await _hotelService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
