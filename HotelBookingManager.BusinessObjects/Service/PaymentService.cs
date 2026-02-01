using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.DTOs;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly HotelBookingContext _context;

        public PaymentService(HotelBookingContext context)
        {
            _context = context;
        }

        // =======================
        // GET ALL
        // =======================
        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Room)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        // =======================
        // GET BY ID
        // =======================
        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Room)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            return payment == null ? null : MapToDto(payment);
        }

        // =======================
        // 🔥 GET BY USER (LỊCH SỬ THANH TOÁN)
        // =======================
        public async Task<IEnumerable<PaymentDto>> GetByUserAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Room)
                .Where(p => p.Booking.UserId == userId)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }




        // =======================
        // ENSURE PAYMENT
        // =======================
        public async Task<Payment> EnsurePaymentForBookingAsync(int bookingId, string method)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment != null)
            {
                if (payment.Status == "Pending")
                {
                    payment.Method = method;
                    await _context.SaveChangesAsync();
                }
                return payment;
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId)
                ?? throw new Exception("Booking không tồn tại");

            payment = new Payment
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                Amount = booking.TotalPrice,
                Method = method,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment;
        }





        // =======================
        // PAY
        // =======================
        public async Task PayAsync(int paymentId, string method)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return;

            payment.Method = method;

            if (method == "VNPAY")
            {
                payment.Status = "Paid";
                payment.PaidAt = DateTime.Now;
                payment.TransactionCode = Guid.NewGuid().ToString("N")[..10];
            }
            // Cash → KHÔNG set Paid

            await _context.SaveChangesAsync();
        }



        // =======================
        // PAY RESULT
        // =======================
        public async Task<PaymentDto?> GetPayResultAsync(int paymentId)
        {
            return await GetByIdAsync(paymentId);
        }

        // =======================
        // MAP DTO
        // =======================
        private static PaymentDto MapToDto(Payment p)
        {
            return new PaymentDto
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                UserId = p.Booking.UserId,
                HotelId = p.Booking?.Room?.HotelId,


                Amount = p.Amount,
                Status = p.Status,
                Method = p.Method,
                TransactionCode = p.TransactionCode,

                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt,

                // 🔥 KHÔNG CÒN "Nguyễn Văn A"
                CustomerName = p.Booking.User.FullName,
                RoomNumber = p.Booking.Room.RoomNumber,
                CheckInDate = p.Booking.CheckInDate,
                CheckOutDate = p.Booking.CheckOutDate
            };
        }
    }
}
