using HotelBookingManager.BusinessObjects.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackDto>> GetAllAsync();
        Task<FeedbackDto?> GetByIdAsync(int id);
        Task<FeedbackDto> CreateAsync(FeedbackDto dto);
        Task UpdateAsync(FeedbackDto dto);
        Task DeleteAsync(int id);
    }
}
