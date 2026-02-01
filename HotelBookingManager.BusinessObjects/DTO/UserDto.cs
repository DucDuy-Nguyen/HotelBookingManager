using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public int? HotelId { get; set; }   // <-- THÊM DÒNG NÀY
    }

}
