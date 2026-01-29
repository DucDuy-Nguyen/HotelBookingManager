using HotelBookingManager.DataAccess.Models;
using System.Threading.Tasks;

namespace HotelBookingManager.DataAccess.IRepositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<int> SaveChangesAsync();
        // thêm method khác nếu cần, ví dụ GetByIdAsync, GetByBookingIdAsync, v.v.
    }
}
