using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomService _roomService;

        public BookingService(
            IBookingRepository bookingRepository,
            IRoomService roomService)
        {
            _bookingRepository = bookingRepository;
            _roomService = roomService;
        }

        // ====== MAPPING ======
        private static BookingDto ToDto(Booking b) => new BookingDto
        {
            BookingId = b.BookingId,
            HotelId = b.HotelId,
            RoomId = b.RoomId,
            UserId = b.UserId,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            Status = b.Status,
            TotalPrice = b.TotalPrice
        };
        public async Task<IEnumerable<BookingDto>> GetByUserAsync(int userId)
        {
            var list = await _bookingRepository.GetByUserAsync(userId);
            return list.Select(ToDto);
        }
        private static Booking ToEntity(BookingDto dto) => new Booking
        {
            BookingId = dto.BookingId,
            HotelId = dto.HotelId,
            RoomId = dto.RoomId,
            UserId = dto.UserId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            Status = dto.Status,
            TotalPrice = dto.TotalPrice
        };

        // ====== IMPLEMENT IBookingService với DTO ======

        public async Task<IEnumerable<BookingDto>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Select(ToDto);
        }

        public async Task<IEnumerable<BookingDto>> GetByStatusAsync(string status)
        {
            var bookings = await _bookingRepository.GetByStatusAsync(status);
            return bookings.Select(ToDto);
        }

        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : ToDto(booking);
        }

        public async Task<int> CreateAsync(BookingDto bookingDto)
        {
            var room = await _roomService.GetByIdAsync(bookingDto.RoomId);
            if (room == null)
                throw new Exception("Room not found");

            var nights = (bookingDto.CheckOutDate.Date - bookingDto.CheckInDate.Date).TotalDays;
            if (nights <= 0) nights = 1;

            var booking = ToEntity(bookingDto);

            booking.TotalPrice = room.CurrentPrice * (decimal)nights;
            booking.Status = "Pending";
            booking.CreatedAt = DateTime.Now;

            room.Status = "Booked";
            await _roomService.UpdateAsync(room);

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return booking.BookingId;
        }

        public async Task ChangeStatusAsync(int bookingId, string newStatus)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return;

            booking.Status = newStatus;
            _bookingRepository.Update(booking);

            if (newStatus == "CheckedOut" || newStatus == "Cancelled")
            {
                var room = await _roomService.GetByIdAsync(booking.RoomId);
                if (room != null)
                {
                    room.Status = "Available";
                    await _roomService.UpdateAsync(room);
                }
            }

            await _bookingRepository.SaveChangesAsync();
        }
    }
}
