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
        private readonly IBookingRepository _bookingRepo;
        private readonly IRoomService _roomService;

        public BookingService(
            IBookingRepository bookingRepo,
            IRoomService roomService)
        {
            _bookingRepo = bookingRepo;
            _roomService = roomService;
        }

        // ================= MAPPING =================
        private static BookingDto ToDto(Booking b) => new BookingDto
        {
            BookingId = b.BookingId,
            HotelId = b.HotelId,
            RoomId = b.RoomId,
            UserId = b.UserId,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            Status = b.Status,
            TotalPrice = b.TotalPrice,
            CustomerName = b.User?.FullName,
            HotelName = b.Hotel?.Name,
            RoomNumber = b.Room?.RoomNumber
        };

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

        // ================= CRUD =================
        public async Task<IEnumerable<BookingDto>> GetAllAsync()
            => (await _bookingRepo.GetAllAsync()).Select(ToDto);

        public async Task<IEnumerable<BookingDto>> GetByStatusAsync(string status)
            => (await _bookingRepo.GetByStatusAsync(status)).Select(ToDto);

        public async Task<IEnumerable<BookingDto>> GetByUserAsync(int userId)
            => (await _bookingRepo.GetByUserAsync(userId)).Select(ToDto);

        public async Task<int> CreateAsync(BookingDto dto)
        {
            var room = await _roomService.GetByIdAsync(dto.RoomId);
            if (room == null) throw new Exception("Room not found");

            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
            if (nights <= 0) nights = 1;

            var booking = ToEntity(dto);
            booking.TotalPrice = room.CurrentPrice * nights;
            booking.Status = "Pending";
            booking.CreatedAt = DateTime.Now;

            room.Status = "Booked";
            await _roomService.UpdateAsync(room);

            await _bookingRepo.AddAsync(booking);
            await _bookingRepo.SaveChangesAsync();

            return booking.BookingId;
        }

        // ⭐ QUAN TRỌNG: DÙNG USER ĐANG LOGIN
        public async Task<int> QuickCreateAsync(int roomId, int hotelId, int days, int userId)
        {
            var room = await _roomService.GetByIdAsync(roomId);
            if (room == null) throw new Exception("Room not found");
            if (room.Status != "Available") throw new Exception("Room not available");

            if (days <= 0) days = 1;

            var booking = new Booking
            {
                HotelId = hotelId,
                RoomId = roomId,
                UserId = userId, // ✅ USER THẬT
                CheckInDate = DateTime.Today.AddDays(1),
                CheckOutDate = DateTime.Today.AddDays(1 + days),
                Status = "PendingPayment",
                TotalPrice = room.CurrentPrice * days,
                CreatedAt = DateTime.Now
            };

            room.Status = "Booked";
            await _roomService.UpdateAsync(room);

            await _bookingRepo.AddAsync(booking);
            await _bookingRepo.SaveChangesAsync();

            return booking.BookingId;
        }

        public async Task ChangeStatusAsync(int bookingId, string newStatus)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return;

            booking.Status = newStatus;
            _bookingRepo.Update(booking);

            if (newStatus == "Cancelled" || newStatus == "CheckedOut")
            {
                var room = await _roomService.GetByIdAsync(booking.RoomId);
                if (room != null)
                {
                    room.Status = "Available";
                    await _roomService.UpdateAsync(room);
                }
            }

            await _bookingRepo.SaveChangesAsync();
        }

    }
}
