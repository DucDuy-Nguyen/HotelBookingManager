using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.Models
{
    public class RoomImage
    {
        public int RoomImageId { get; set; }
        public int RoomId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsThumbnail { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Room Room { get; set; } = null!;
    }
}
