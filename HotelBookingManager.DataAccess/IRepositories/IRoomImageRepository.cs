using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.IRepositories
{
    public interface IRoomImageRepository
    {
        Task<IEnumerable<RoomImage>> GetByRoomIdAsync(int roomId);
        Task<RoomImage?> GetThumbnailAsync(int roomId);

        Task AddAsync(RoomImage image);
        void Update(RoomImage image);
        void Delete(RoomImage image);

        Task<int> SaveChangesAsync();
        Task<RoomImage?> GetByIdAsync(int id);
        Task AddRangeAsync(IEnumerable<RoomImage> images);

    }
}
