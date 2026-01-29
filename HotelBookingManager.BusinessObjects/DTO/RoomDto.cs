namespace HotelBookingManager.BusinessObjects.DTO
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomNumber { get; set; } = "";
        public int? Floor { get; set; }
        public string Status { get; set; } = "";
        public decimal CurrentPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
