using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomType>> GetAllAsync();

    }
}
