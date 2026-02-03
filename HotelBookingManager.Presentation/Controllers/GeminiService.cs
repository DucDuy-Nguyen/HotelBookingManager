using GenerativeAI;
using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using System.Text.Json;

namespace HotelBookingManager.Presentation.Controllers
{
    public class GeminiService : IGeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException(
                    "Gemini:ApiKey không được tìm thấy trong appsettings.json. " +
                    "Vui lòng thêm: \"Gemini\": { \"ApiKey\": \"YOUR_API_KEY\" }");
            }
        }

        public async Task<string> GetHotelAndRoomRecommendationsAsync(
            IEnumerable<HotelDto> allHotels,
            IEnumerable<RoomDto> allRooms,
            string customerPreferences)
        {
            try
            {
                // Chuẩn bị dữ liệu hotel
                var hotelsList = allHotels.Select(h => new
                {
                    h.HotelId,
                    h.Name,
                    h.Address,
                    h.City,
                    h.Country,
                    h.Rating,
                    h.IsActive
                }).ToList();

                // Chuẩn bị dữ liệu phòng
                var roomsList = allRooms.Select(r => new
                {
                    r.RoomId,
                    r.RoomNumber,
                    r.HotelId,
                    r.RoomTypeName,
                    r.CurrentPrice,
                    r.Floor,
                    r.Status
                }).ToList();

                var hotelsJson = JsonSerializer.Serialize(
                    hotelsList,
                    new JsonSerializerOptions { WriteIndented = true });

                var roomsJson = JsonSerializer.Serialize(
                    roomsList,
                    new JsonSerializerOptions { WriteIndented = true });

                var prompt = $@"
Bạn là một trợ lý AI giúp khách hàng tìm hotel và phòng phù hợp nhất với yêu cầu của họ.

DANH SÁCH TẤT CẢ CÁC KHÁCH SẠN CÓ SẴN:
{hotelsJson}

DANH SÁCH TẤT CẢ CÁC PHÒNG CÓ SẴN:
{roomsJson}

YÊU CẦU CỦA KHÁCH HÀNG:
{customerPreferences}

HƯỚNG DẪN:
1. Phân tích yêu cầu của khách hàng một cách chi tiết
2. Dựa trên yêu cầu, tìm hotel(s) phù hợp nhất
3. Với mỗi hotel được chọn, tìm các phòng phù hợp nhất
4. Đề xuất TOP 3 GIẢI PHÁP TỐT NHẤT (mỗi giải pháp gồm 1 hotel + 1-2 phòng)
5. Giải thích rõ lý do tại sao mỗi giải pháp phù hợp
6. Liệt kê chi tiết: Tên hotel, địa chỉ, phòng, giá, vị trí phòng (tầng)
7. Trả lời bằng tiếng Việt

ĐỊNH DẠNG RESPONSE:
Sử dụng HTML với các tag <h3>, <h4>, <p>, <ul>, <li>, <strong> để dễ hiển thị trên web.
Không sử dụng markdown, chỉ dùng HTML thuần.

GỢI Ý STRUCTURE:
<h3>✨ Đề Xuất #1: Tên Hotel</h3>
<p><strong>Địa chỉ:</strong> ...</p>
<p><strong>Đánh giá:</strong> ⭐ ...</p>
<h4>Phòng Gợi Ý:</h4>
<ul>
  <li><strong>Phòng #101</strong> - Loại phòng - Tầng 1 - <strong>Giá: XXX VNĐ</strong></li>
  <li>Lý do: ...</li>
</ul>
<p><strong>Tổng thể:</strong> ...</p>
";

                // Dùng GoogleAi class
                var googleAI = new GoogleAi(_apiKey);
                var model = googleAI.CreateGenerativeModel("gemini-pro");

                // Gọi API
                var response = await model.GenerateContentAsync(prompt);

                // Lấy text từ response
                var result = response?.Text();

                if (string.IsNullOrEmpty(result))
                {
                    return "❌ Không thể tạo đề xuất lúc này. Vui lòng thử lại sau.";
                }

                return result;
            }
            catch (ArgumentException ex) when (ex.Message.Contains("API key"))
            {
                return $"❌ Lỗi API key: {ex.Message}<br/>Vui lòng kiểm tra appsettings.json";
            }
            catch (HttpRequestException ex)
            {
                return $"❌ Lỗi kết nối tới Gemini API: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"❌ Lỗi khi xử lý yêu cầu: {ex.Message}";
            }
        }
    }
}
