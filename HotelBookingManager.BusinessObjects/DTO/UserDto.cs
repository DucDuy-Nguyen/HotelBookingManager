using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
    }
}
