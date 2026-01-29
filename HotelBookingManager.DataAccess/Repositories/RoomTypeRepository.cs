using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly HotelBookingContext _context;

        public RoomTypeRepository(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomType>> GetAllAsync()
        {
            return await _context.RoomTypes.ToListAsync();
        }
    }
}
