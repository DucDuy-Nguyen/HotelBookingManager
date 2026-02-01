namespace HotelBookingManager.BusinessObjects.DTOs
{
    public class PaymentListDTO
    {
        public string? TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "";
        public string Status { get; set; } = "";

        public string? RoomNumber { get; set; }
        public string? CustomerName { get; set; }
    }
}
