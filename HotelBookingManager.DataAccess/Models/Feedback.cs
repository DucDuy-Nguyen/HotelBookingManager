using HotelBookingManager.DataAccess.Models;

public class Feedback
{
    public int FeedbackId { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int HotelId { get; set; }
    public Hotel Hotel { get; set; }

    public int Rating { get; set; }   // 1–5
    public string Comment { get; set; }

    public bool IsApproved { get; set; } // Admin duyệt
    public DateTime CreatedAt { get; set; }
}
