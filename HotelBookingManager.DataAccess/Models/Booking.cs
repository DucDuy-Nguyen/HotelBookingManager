namespace HotelBookingManager.DataAccess.Models;

public class Booking
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public User? User { get; set; } = null!;
    public Hotel? Hotel { get; set; } = null!;
    public Room? Room { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
