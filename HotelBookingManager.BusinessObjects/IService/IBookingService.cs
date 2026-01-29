using HotelBookingManager.BusinessObjects.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllAsync();
        Task<IEnumerable<BookingDto>> GetByStatusAsync(string status);
        Task<int> CreateAsync(BookingDto booking);

        Task ChangeStatusAsync(int bookingId, string newStatus);
        Task<IEnumerable<BookingDto>> GetByUserAsync(int userId);


    }
}
