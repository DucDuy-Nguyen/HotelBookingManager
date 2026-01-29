using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelBookingContext _context;
        private readonly DbSet<Room> _dbSet;

        public RoomRepository(HotelBookingContext context)
        {
            _context = context;
            _dbSet = _context.Rooms;
        }

        // Lấy tất cả phòng (cho admin)
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .ToListAsync();
        }
        public async Task<IEnumerable<Room>> GetByStatusAsync(string status)
        {
            var query = _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .AsQueryable();   // [web:227]

            switch (status?.ToLower())
            {
                case "active":
                    query = query.Where(r => r.IsActive);
                    break;
                case "inactive":
                    query = query.Where(r => !r.IsActive);
                    break;
                case "all":
                default:
                    // không filter
                    break;
            }

            return await query.ToListAsync();
        }

        // Lấy phòng theo hotel (cho user xem theo khách sạn)
        public async Task<IEnumerable<Room>> GetByHotelAsync(int hotelId, bool onlyAvailable)
        {
            var query = _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotelId && r.IsActive);

            if (onlyAvailable)
            {
                query = query.Where(r => r.Status == "Available");
            }

            return await query.ToListAsync();
        }

        // Lấy 1 phòng theo Id (cho Edit/Delete/Details)
        public Task<Room?> GetByIdAsync(int id)
        {
            return _dbSet
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task AddAsync(Room room)
        {
            await _dbSet.AddAsync(room);
        }

        public void Update(Room room)
        {
            _dbSet.Update(room);
        }

        public void Delete(Room room)
        {
            _dbSet.Remove(room);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
