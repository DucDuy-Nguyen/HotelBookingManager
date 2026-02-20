using HotelBookingManager.BusinessObjects.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IGeminiService
    {
        Task<string> GetHotelAndRoomRecommendationsAsync(
            IEnumerable<HotelDto> allHotels,
            IEnumerable<RoomDto> allRooms,
            string customerPreferences
        );

    }
}
