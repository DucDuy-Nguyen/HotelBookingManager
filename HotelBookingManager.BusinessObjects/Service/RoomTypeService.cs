using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeService(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        public Task<IEnumerable<RoomType>> GetAllAsync()
            => _roomTypeRepository.GetAllAsync();

    }
}
