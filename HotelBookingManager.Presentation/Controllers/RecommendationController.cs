using HotelBookingManager.BusinessObjects.IService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingManager.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IRoomService _roomService;
        private readonly IGeminiService _geminiService;

        public RecommendationController(
            IHotelService hotelService,
            IRoomService roomService,
            IGeminiService geminiService)
        {
            _hotelService = hotelService;
            _roomService = roomService;
            _geminiService = geminiService;
        }

        /// <summary>
        /// Lấy đề xuất hotel và phòng từ Gemini AI dựa trên yêu cầu của user
        /// </summary>
        [HttpPost("get-suggestions")]
        public async Task<IActionResult> GetSuggestions(
            [FromBody] RecommendationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Preferences))
            {
                return BadRequest(new { message = "Yêu cầu không được bỏ trống" });
            }

            try
            {
                // Lấy TẤT CẢ hotel
                var allHotels = await _hotelService.GetAllAsync();

                if (!allHotels.Any())
                {
                    return NotFound(new
                    {
                        message = "Hiện không có khách sạn nào trong hệ thống"
                    });
                }

                // Lấy TẤT CẢ phòng
                var allRooms = await _roomService.GetAllAsync();

                if (!allRooms.Any())
                {
                    return NotFound(new
                    {
                        message = "Hiện không có phòng nào trong hệ thống"
                    });
                }

                // Gọi Gemini để tạo đề xuất
                var recommendations = await _geminiService.GetHotelAndRoomRecommendationsAsync(
                    allHotels,
                    allRooms,
                    request.Preferences);

                return Ok(new
                {
                    success = true,
                    recommendations = recommendations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi máy chủ: {ex.Message}"
                });
            }
        }
    }

    public class RecommendationRequest
    {
        public string Preferences { get; set; }
    }
}
