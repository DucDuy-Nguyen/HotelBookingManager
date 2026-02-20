using HotelBookingManager.BusinessObjects.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IRoomImageService
    {
        Task<IEnumerable<RoomImageDto>> GetByRoomIdAsync(int roomId);
        Task<RoomImageDto?> GetThumbnailAsync(int roomId);

        Task<RoomImageDto> CreateAsync(int roomId, RoomImageDto dto);
        Task<bool> SetThumbnailAsync(int roomImageId);
        Task<bool> DeleteAsync(int roomImageId);
        Task AddImagesAsync(int roomId, List<string> imageUrls);
        Task<RoomImageDto?> GetByIdAsync(int id);


    }
}
