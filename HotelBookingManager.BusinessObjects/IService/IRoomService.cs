using HotelBookingManager.BusinessObjects.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllAsync();
        Task<RoomDto?> GetByIdAsync(int id);
        Task<IEnumerable<RoomDto>> GetByHotelAsync(int hotelId, bool onlyAvailable);
        Task<IEnumerable<RoomDto>> GetByStatusAsync(string status);
        Task<RoomDto> CreateAsync(RoomDto room);
        Task<bool> UpdateAsync(RoomDto room);
        Task<bool> DeleteAsync(int id);
    }
}
