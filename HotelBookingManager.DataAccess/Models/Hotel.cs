namespace HotelBookingManager.DataAccess.Models;

public class Hotel
{
    public int HotelId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public decimal? Rating { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
