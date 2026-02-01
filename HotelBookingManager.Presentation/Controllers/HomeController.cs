using System.Diagnostics;
using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingManager.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHotelService _hotelService;

        public HomeController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy 6 khách sạn active (hoặc tất cả, tuỳ bạn)
            var hotels = (await _hotelService.GetFilteredAsync(null, "active"))
                         .Take(10)
                         .ToList();

            return View(hotels); // model: IEnumerable<HotelDto>
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}