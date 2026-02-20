using System.ComponentModel.DataAnnotations;
using HotelBookingManager.BusinessObjects.DTO;
using Microsoft.AspNetCore.Http;

namespace HotelBookingManager.Presentation.Models
{
    public class RoomCreateEditViewModel
    {
        public int? RoomId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách sạn")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại phòng")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số phòng")]
        [StringLength(20, ErrorMessage = "Số phòng quá dài")]
        public string RoomNumber { get; set; } = null!;

        [Range(0, 100, ErrorMessage = "Tầng phải từ 0-100")]
        public int Floor { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá phòng")]
        [Range(0, 100000000, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal CurrentPrice { get; set; }

        // ⭐ UPLOAD MỚI
        [Display(Name = "Thêm ảnh mới")]
        public List<IFormFile>? Images { get; set; } = new();

        // 🆕 THÊM CHO EDIT: Hiển thị ảnh hiện tại
        public List<RoomImageDto>? RoomImages { get; set; } = new();
        public string? HotelName { get; set; }
        public string? RoomTypeName { get; set; }
        public string Status { get; set; } = "Available";
        public bool IsActive { get; set; } = true;
    }
}
