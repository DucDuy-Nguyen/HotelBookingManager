namespace HotelBookingManager.Presentation.Models
{
    public class HotelViewModel
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
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }  // thêm

    }

    public class HotelSearchViewModel
    {
        public string? City { get; set; }
        public string StatusFilter { get; set; } = "active";

        public List<HotelViewModel> Hotels { get; set; } = new();
    }
}