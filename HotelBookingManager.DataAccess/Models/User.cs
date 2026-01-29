using System.Data;

namespace HotelBookingManager.DataAccess.Models;

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string? Gender { get; set; }
    public string? IdentityNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public Role? Role { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
