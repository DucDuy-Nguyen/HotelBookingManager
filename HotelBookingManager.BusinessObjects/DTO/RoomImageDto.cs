using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.DTO
{
    public class RoomImageDto
    {
        public int RoomImageId { get; set; }
        public int RoomId { get; set; }     // ← THÊM

        public string ImageUrl { get; set; } = "";
        public int DisplayOrder { get; set; }
        public bool IsThumbnail { get; set; }
    }
}

