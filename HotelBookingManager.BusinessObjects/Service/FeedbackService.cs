using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepo;

        public FeedbackService(IFeedbackRepository feedbackRepo)
        {
            _feedbackRepo = feedbackRepo;
        }

        private static FeedbackDto ToDto(Feedback f)
        {
            return new FeedbackDto
            {
                FeedbackId = f.FeedbackId,
                BookingId = f.BookingId,
                UserId = f.UserId,
                HotelId = f.HotelId,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                CustomerName = f.User?.FullName,
                HotelName = f.Hotel?.Name,
                RoomNumber = f.Booking?.Room?.RoomNumber
            };
        }

        public async Task<IEnumerable<FeedbackDto>> GetAllAsync()
        {
            var list = await _feedbackRepo.GetAllAsync();
            return list.Select(ToDto);
        }

        public async Task<FeedbackDto?> GetByIdAsync(int id)
        {
            var f = await _feedbackRepo.GetByIdAsync(id);
            return f == null ? null : ToDto(f);
        }

        public async Task<FeedbackDto> CreateAsync(FeedbackDto dto)
        {
            var now = DateTime.Now;

            var entity = new Feedback
            {
                BookingId = dto.BookingId,
                UserId = dto.UserId,
                HotelId = dto.HotelId,
                Rating = dto.Rating,
                Comment = dto.Comment ?? "",
                CreatedAt = now,
                UpdatedAt = null
            };

            await _feedbackRepo.AddAsync(entity);
            await _feedbackRepo.SaveChangesAsync();

            var saved = await _feedbackRepo.GetByIdAsync(entity.FeedbackId) ?? entity;
            return ToDto(saved);
        }

        public async Task UpdateAsync(FeedbackDto dto)
        {
            var f = await _feedbackRepo.GetByIdAsync(dto.FeedbackId);
            if (f == null) throw new Exception("Feedback not found");

            f.Rating = dto.Rating;
            f.Comment = dto.Comment ?? "";
            f.UpdatedAt = DateTime.Now;

            await _feedbackRepo.UpdateAsync(f);
            await _feedbackRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _feedbackRepo.DeleteAsync(id);
            await _feedbackRepo.SaveChangesAsync();
        }
    }
}
