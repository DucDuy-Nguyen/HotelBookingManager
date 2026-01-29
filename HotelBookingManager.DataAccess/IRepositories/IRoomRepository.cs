using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.IRepositories
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<IEnumerable<Room>> GetByHotelAsync(int hotelId, bool onlyAvailable);
        Task<Room?> GetByIdAsync(int id);

        Task AddAsync(Room room);
        void Update(Room room);
        void Delete(Room room);         
        Task<int> SaveChangesAsync();
        Task<IEnumerable<Room>> GetByStatusAsync(string status);


    }
}
