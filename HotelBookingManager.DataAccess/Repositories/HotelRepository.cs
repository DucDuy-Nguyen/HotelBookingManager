using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelBookingContext _context;
        private readonly DbSet<Hotel> _dbSet;

        public HotelRepository(HotelBookingContext context)
        {
            _context = context;
            _dbSet = _context.Hotels;
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            return await _dbSet
                .ToListAsync();
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(h => h.HotelId == id);
        }

        public async Task AddAsync(Hotel hotel)
        {
            await _dbSet.AddAsync(hotel);
        }

        public void Update(Hotel hotel)
        {
            _dbSet.Update(hotel);
        }

        public void Delete(Hotel hotel)
        {
            _dbSet.Remove(hotel);
        }

        public async Task<IEnumerable<Hotel>> SearchByCityAsync(string city)
        {
            city = city.Trim();
            return await _dbSet
                .Where(h => h.IsActive && h.City.Contains(city))
                .ToListAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
