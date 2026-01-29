using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HotelBookingContext _context;

        public BookingRepository(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Hotel)
                .Include(b => b.Room)
                .Include(b => b.Payments)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByStatusAsync(string status)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Hotel)
                .Include(b => b.Room)
                .Include(b => b.Payments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(b => b.Status == status);
            }

            return await query.ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Hotel)
                .Include(b => b.Room)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
        }

        public void Delete(Booking booking)
        {
            _context.Bookings.Remove(booking);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
