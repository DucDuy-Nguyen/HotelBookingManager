using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingManager.Presentation.Controllers
{
    [Authorize]  // Cần login
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // Helper: Check RoleId 1(Admin) hoặc 2(Staff)
        private bool IsAdminOrStaff()
        {
            var roleClaim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(roleClaim, out int roleId) && (roleId == 1 || roleId == 2);
        }

        // Public: ai cũng xem
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? city, string? statusFilter)
        {
            statusFilter ??= "active";
            var hotels = await _hotelService.GetFilteredAsync(city, statusFilter);

            var model = new HotelSearchViewModel
            {
                City = city,
                StatusFilter = statusFilter,
                Hotels = hotels.Select(h => new HotelViewModel
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    Address = h.Address,
                    City = h.City,
                    Country = h.Country,
                    Rating = (decimal?)h.Rating,
                    IsActive = h.IsActive
                }).ToList()
            };

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null) return NotFound();

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
                IsActive = hotel.IsActive
            };

            return View(model);
        }

        // ADMIN/STAFF ONLY (RoleId 1,2)
        public IActionResult Create()
        {
            if (!IsAdminOrStaff()) return Forbid();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelViewModel model)
        {
            if (!IsAdminOrStaff()) return Forbid();
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
                IsActive = model.IsActive
            };

            await _hotelService.CreateAsync(hotelDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdminOrStaff()) return Forbid();
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null) return NotFound();

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
                IsActive = hotel.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelViewModel model)
        {
            if (!IsAdminOrStaff()) return Forbid();
            if (id != model.HotelId) return BadRequest();
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
                IsActive = model.IsActive
            };

            var ok = await _hotelService.UpdateAsync(hotelDto);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminOrStaff()) return Forbid();
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null) return NotFound();

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
                IsActive = hotel.IsActive
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminOrStaff()) return Forbid();
            await _hotelService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
