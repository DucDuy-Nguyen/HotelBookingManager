namespace HotelBookingManager.BusinessObjects.DTO
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int? HotelId { get; set; }     // để lọc theo staff

        public decimal Amount { get; set; }
        public string Status { get; set; } = "";
        public string Method { get; set; } = "";
        public string? TransactionCode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        public string? CustomerName { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
    }
}
