using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly HotelBookingContext _context;

        public PaymentRepository(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
