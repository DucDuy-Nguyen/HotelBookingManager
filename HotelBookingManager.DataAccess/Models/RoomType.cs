namespace HotelBookingManager.DataAccess.Models;

public class RoomType
{
    public int RoomTypeId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public int MaxAdults { get; set; }
    public int MaxChildren { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
