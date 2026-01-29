using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly HotelBookingContext _context;

        public FeedbackRepository(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Hotel)
                .Include(f => f.Booking).ThenInclude(b => b.Room)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Hotel)
                .Include(f => f.Booking).ThenInclude(b => b.Room)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);
        }

        public async Task AddAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var f = await _context.Feedbacks.FindAsync(id);
            if (f != null)
            {
                _context.Feedbacks.Remove(f);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
