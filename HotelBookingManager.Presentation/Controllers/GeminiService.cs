using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _apiKey = configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Thiếu Gemini:ApiKey");

        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> GetHotelAndRoomRecommendationsAsync(
        IEnumerable<HotelDto> allHotels,
        IEnumerable<RoomDto> allRooms,
        string customerPreferences)
    {
        var hotelsJson = JsonSerializer.Serialize(allHotels);
        var roomsJson = JsonSerializer.Serialize(allRooms);

        var prompt = $@"
Bạn là trợ lý AI giúp khách hàng chọn khách sạn và phòng phù hợp nhất.

DANH SÁCH KHÁCH SẠN (CÓ HotelId):
{hotelsJson}

DANH SÁCH PHÒNG (CÓ RoomId, HotelId):
{roomsJson}

YÊU CẦU KHÁCH HÀNG:
{customerPreferences}

NHIỆM VỤ:
1. Phân tích yêu cầu khách hàng
2. Chọn TOP 3 GIẢI PHÁP TỐT NHẤT
3. Mỗi giải pháp gồm:
   - 1 khách sạn
   - 1 phòng cụ thể từ danh sách trên

⚠️ BẮT BUỘC:
- CHỈ sử dụng RoomId và HotelId có trong dữ liệu
- TUYỆT ĐỐI KHÔNG bịa ID
- Với mỗi phòng, hiển thị HAI NÚT như sau:

<div class='d-flex gap-2 mt-2'>
    <button class='btn btn-success book-room'
            data-room-id='ROOM_ID'
            data-hotel-id='HOTEL_ID'>
        Đặt phòng ngay
    </button>

    <button class='btn btn-outline-primary room-detail'
            data-room-id='ROOM_ID'>
        Xem chi tiết
    </button>
</div>

ĐỊNH DẠNG KẾT QUẢ:
- Trả về HTML THUẦN
- KHÔNG markdown
- Dùng <h3>, <p>, <ul>, <li>, <strong>, <button>
- Viết bằng tiếng Việt
";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}"
        )
        {
            Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"❌ Gemini API lỗi: {error}";
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()
            ?? "❌ Gemini không trả về nội dung";
    }
}