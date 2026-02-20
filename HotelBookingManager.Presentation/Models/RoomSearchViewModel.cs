using HotelBookingManager.BusinessObjects.DTO;

public class RoomSearchViewModel
{
    public string? HotelName { get; set; }
    public string? StatusFilter { get; set; }
    public IEnumerable<RoomDto> Rooms { get; set; } = Enumerable.Empty<RoomDto>();
}
