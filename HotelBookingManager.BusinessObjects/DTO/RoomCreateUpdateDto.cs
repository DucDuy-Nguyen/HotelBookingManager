using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.Generic;


namespace HotelBookingManager.BusinessObjects.DTO
{
    public class RoomCreateUpdateDto
    {
        public int? RoomId { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        public string RoomNumber { get; set; } = null!;

        public int Floor { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
