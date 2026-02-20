namespace HotelBookingManager.Presentation.Models
{
    public class UserManageViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public int? HotelId { get; set; }
        public string? HotelName { get; set; }   // thêm

        public bool IsActive { get; set; }
    }
}
