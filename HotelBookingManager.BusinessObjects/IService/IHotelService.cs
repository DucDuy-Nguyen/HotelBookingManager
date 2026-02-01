using HotelBookingManager.BusinessObjects.DTO;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetAllAsync();

        Task<HotelDto?> GetByIdAsync(int id);

        Task<HotelDto> CreateAsync(HotelDto hotel);

        Task<bool> UpdateAsync(HotelDto hotel);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<HotelDto>> SearchByCityAsync(string city);

        Task<IEnumerable<HotelDto>> GetFilteredAsync(string? city, string statusFilter);
    }
}