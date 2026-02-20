namespace HotelBookingManager.BusinessObjects.DTO
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public string? HotelName { get; set; }
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public string RoomNumber { get; set; } = "";
        public int? Floor { get; set; }
        public string Status { get; set; } = "Available";
        public decimal CurrentPrice { get; set; }
        public bool IsActive { get; set; }

        // 🆕 THÊM 3 PROPERTIES NÀY:
        public string? FirstImageUrl { get; set; }
        public int RoomImagesCount { get; set; }
        public List<RoomImageDto>? RoomImages { get; set; } = new List<RoomImageDto>();
    }
}
