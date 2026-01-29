using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.DataAccess.IRepositories
{
    public interface IRoomTypeRepository
    {
        Task<IEnumerable<RoomType>> GetAllAsync();
    }
}
