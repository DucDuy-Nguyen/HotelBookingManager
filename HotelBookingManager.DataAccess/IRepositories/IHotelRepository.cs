using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.IRepositories
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> GetAllAsync();
        Task<Hotel?> GetByIdAsync(int id);
        Task AddAsync(Hotel hotel);
        void Update(Hotel hotel);
        void Delete(Hotel hotel);
        Task<IEnumerable<Hotel>> SearchByCityAsync(string city);
        Task<int> SaveChangesAsync();
    }
}