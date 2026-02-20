using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class RoomImageRepository : IRoomImageRepository
    {
        private readonly HotelBookingContext _context;

        public RoomImageRepository(HotelBookingContext context)
        {
            _context = context;
        }
        public Task<RoomImage?> GetByIdAsync(int id)
        {
            return _context.RoomImages
                .FirstOrDefaultAsync(i => i.RoomImageId == id && i.IsActive);
        }
        public async Task AddRangeAsync(IEnumerable<RoomImage> images)
        {
            await _context.RoomImages.AddRangeAsync(images);
            await _context.SaveChangesAsync(); // 🔥 BẮT BUỘC

        }
        // Trong RoomImageRepository.cs
        public async Task<IEnumerable<RoomImage>> GetByRoomIdAsync(int roomId)
        {
            return await _context.RoomImages
                .Where(ri => ri.RoomId == roomId && ri.IsActive)
                .OrderBy(ri => ri.DisplayOrder)
                .ToListAsync();
        }


        public async Task<RoomImage?> GetThumbnailAsync(int roomId)
        {
            return await _context.RoomImages
                .FirstOrDefaultAsync(i =>
                    i.RoomId == roomId &&
                    i.IsThumbnail &&
                    i.IsActive);
        }

        public async Task AddAsync(RoomImage image)
        {
            await _context.RoomImages.AddAsync(image);
        }

        public void Update(RoomImage image)
        {
            _context.RoomImages.Update(image);
        }

        public void Delete(RoomImage image)
        {
            _context.RoomImages.Remove(image);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
