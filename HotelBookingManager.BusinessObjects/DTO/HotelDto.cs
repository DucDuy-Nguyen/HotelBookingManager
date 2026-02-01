using System;

namespace HotelBookingManager.BusinessObjects.DTO
{
    public class HotelDto
    {
        public int HotelId { get; set; }

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public string Address { get; set; } = "";

        public string City { get; set; } = "";

        public string Country { get; set; } = "";

        public double? Rating { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; } // để hiển thị ảnh

    }
}