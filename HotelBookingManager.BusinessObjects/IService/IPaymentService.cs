using HotelBookingManager.BusinessObjects.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDto?> GetByIdAsync(int id);
        Task<PaymentDto> EnsurePaymentForBookingAsync(int bookingId);
        Task PayAsync(int paymentId, string method);
        Task<PaymentDto?> GetPayResultAsync(int paymentId);
    }
}
