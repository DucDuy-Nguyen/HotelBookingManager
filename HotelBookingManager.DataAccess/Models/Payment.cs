namespace HotelBookingManager.DataAccess.Models;


    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }

        public decimal Amount { get; set; }
        

        public string Status { get; set; } = null!; // Success | Pending
   
    public string? Method { get; set; }
    public string? TransactionCode { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Booking Booking { get; set; }

    public User User { get; set; } = null!;
}
