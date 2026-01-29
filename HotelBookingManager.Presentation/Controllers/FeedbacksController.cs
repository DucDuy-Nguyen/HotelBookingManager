using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class FeedbacksController : Controller
{
    private readonly IFeedbackService _feedbackService;
    private readonly IHotelService _hotelService;
    private readonly IBookingService _bookingService;

    public FeedbacksController(
        IFeedbackService feedbackService,
        IHotelService hotelService,
        IBookingService bookingService)
    {
        _feedbackService = feedbackService;
        _hotelService = hotelService;
        _bookingService = bookingService;
    }

    // LIST
    public async Task<IActionResult> Index()
    {
        var list = await _feedbackService.GetAllAsync();
        return View(list);   // IEnumerable<FeedbackDto>
    }

    // DETAILS
    public async Task<IActionResult> Details(int id)
    {
        var f = await _feedbackService.GetByIdAsync(id);
        if (f == null) return NotFound();
        return View(f);      // FeedbackDto
    }

    // CREATE GET
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Challenge();

        var hotels = await _hotelService.GetAllAsync();
        var bookings = await _bookingService.GetByUserAsync(userId.Value);

        var vm = new FeedbackCreateViewModel
        {
            Feedback = new FeedbackDto
            {
                UserId = userId.Value,
                Rating = 5
            },
            Hotels = hotels ?? Enumerable.Empty<HotelDto>(),
            Bookings = bookings ?? Enumerable.Empty<BookingDto>()
        };

        return View(vm);
    }

    // CREATE POST
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeedbackCreateViewModel vm)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Challenge();

        vm.Feedback.UserId = userId.Value;

        if (!ModelState.IsValid)
        {
            vm.Hotels = await _hotelService.GetAllAsync();
            vm.Bookings = await _bookingService.GetByUserAsync(userId.Value);
            return View(vm);
        }

        try
        {
            await _feedbackService.CreateAsync(vm.Feedback);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            vm.Hotels = await _hotelService.GetAllAsync();
            vm.Bookings = await _bookingService.GetByUserAsync(userId.Value);
            return View(vm);
        }
    }

    // EDIT GET
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var f = await _feedbackService.GetByIdAsync(id);
        if (f == null) return NotFound();
        return View(f);   // FeedbackDto
    }

    // EDIT POST
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FeedbackDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _feedbackService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // DELETE POST
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _feedbackService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // Helper
    private int? GetCurrentUserId()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return null;

        var userIdStr = User.FindFirst("UserId")?.Value
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdStr, out var id) ? id : (int?)null;
    }
}
