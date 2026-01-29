using System;
using System.Collections.Generic;
using System.Text;
using HotelBookingManager.DataAccess.Models;

namespace HotelBookingManager.DataAccess.IRepositories
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(int id);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
