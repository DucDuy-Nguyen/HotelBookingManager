using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.DataAccess.Models;
using System.Threading.Tasks;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync();
    Task<PaymentDto?> GetByIdAsync(int id);
    Task<IEnumerable<PaymentDto>> GetByUserAsync(int userId);

    // 🔥 CHỈNH Ở ĐÂY
    Task<Payment> EnsurePaymentForBookingAsync(int bookingId, string method);

    Task PayAsync(int paymentId, string method);
    Task<PaymentDto?> GetPayResultAsync(int paymentId);
}
