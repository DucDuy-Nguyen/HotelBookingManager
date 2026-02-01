using HotelBookingManager.BusinessObjects.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllAsync();
        Task<IEnumerable<BookingDto>> GetByStatusAsync(string status);
        Task<IEnumerable<BookingDto>> GetByUserAsync(int userId);

        Task<int> CreateAsync(BookingDto bookingDto);
        Task<int> QuickCreateAsync(int roomId, int hotelId, int days, int userId);

        Task ChangeStatusAsync(int bookingId, string newStatus);
    }
}
