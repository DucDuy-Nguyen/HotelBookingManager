// BusinessObjects/DTO/FeedbackDto.cs
namespace HotelBookingManager.BusinessObjects.DTO
{
    public class FeedbackDto
    {
        public int FeedbackId { get; set; }

        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int HotelId { get; set; }

        public int Rating { get; set; }          // 1–5
        public string Comment { get; set; } = "";

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? CustomerName { get; set; }
        public string? HotelName { get; set; }
        public string? RoomNumber { get; set; }
    }
}
