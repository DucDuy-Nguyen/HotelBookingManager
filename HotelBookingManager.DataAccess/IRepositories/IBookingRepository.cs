using HotelBookingManager.DataAccess.Models;

namespace HotelBookingManager.DataAccess.IRepositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByStatusAsync(string status);
        Task<Booking?> GetByIdAsync(int id);

        Task AddAsync(Booking booking);
        void Update(Booking booking);
        void Delete(Booking booking);

        Task<int> SaveChangesAsync();
    }
}
