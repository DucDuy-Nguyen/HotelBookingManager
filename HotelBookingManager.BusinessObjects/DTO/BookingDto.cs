using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.DTO
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public string? CustomerName { get; set; }   // User.FullName
        public string? HotelName { get; set; }      // Hotel.Name
        public string? RoomNumber { get; set; }     // Room.RoomNumber
    }
}