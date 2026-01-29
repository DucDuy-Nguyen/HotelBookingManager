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

        // GET: /Hotels
        public async Task<IActionResult> Index(string? city, string? statusFilter)
        {
            statusFilter ??= "active";

            var hotels = await _hotelService.GetFilteredAsync(city, statusFilter); // IEnumerable<HotelDto>

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
                    IsActive = h.IsActive
                }).ToList()
            };

            return View(model);
        }


        // GET: /Hotels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id); // HotelDto?
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

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelViewModel model)
        {
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


        // GET
        public async Task<IActionResult> Edit(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id); // HotelDto?
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

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelViewModel model)
        {
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


        // GET: /Hotels/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id); // HotelDto?
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


        // POST: /Hotels/Delete/5
        // XÓA MỀM: IsActive = false
       [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    await _hotelService.DeleteAsync(id);
    return RedirectToAction(nameof(Index));
}

    }
}
