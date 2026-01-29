namespace HotelBookingManager.DataAccess.Models;

public class Room
{
    public int RoomId { get; set; }
    public int HotelId { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = null!;
    public int? Floor { get; set; }
    public string Status { get; set; } = "Available";
    public decimal CurrentPrice { get; set; }
    public bool IsActive { get; set; }

    public Hotel? Hotel { get; set; } = null!;
    public RoomType? RoomType { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
