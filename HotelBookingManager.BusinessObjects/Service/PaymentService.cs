using HotelBookingManager.BusinessObjects.DTO;
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

        private static PaymentDto ToDto(Payment p)
        {
            return new PaymentDto
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                UserId = p.UserId,
                Amount = p.Amount,
                Status = p.Status,
                Method = p.Method,
                TransactionCode = p.TransactionCode,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt,
                CustomerName = p.User?.FullName,
                RoomNumber = p.Booking?.Room?.RoomNumber,
                CheckInDate = p.Booking?.CheckInDate,
                CheckOutDate = p.Booking?.CheckOutDate
            };
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _context.Payments
                .Include(p => p.Booking).ThenInclude(b => b.Room)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return payments.Select(ToDto);
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking).ThenInclude(b => b.Room)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            return payment == null ? null : ToDto(payment);
        }

        public async Task<PaymentDto> EnsurePaymentForBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null)
            {
                payment = new Payment
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    Amount = booking.TotalPrice,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    Method = "Cash"
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }

            payment = await _context.Payments
                .Include(p => p.Booking).ThenInclude(b => b.Room)
                .Include(p => p.User)
                .FirstAsync(p => p.PaymentId == payment.PaymentId);

            return ToDto(payment);
        }

        public async Task PayAsync(int paymentId, string method)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                throw new Exception("Payment not found");

            payment.Method = method;

            if (method == "Cash")
            {
                payment.Status = "Pending";
                payment.TransactionCode = null;
                payment.PaidAt = null;
            }
            else
            {
                payment.Status = "Success";
                payment.TransactionCode = "TXN" + DateTime.Now.Ticks;
                payment.PaidAt = DateTime.Now;
            }

            if (payment.Booking.Status == "Pending")
            {
                payment.Booking.Status = "Confirmed";
            }

            await _context.SaveChangesAsync();
        }

        public async Task<PaymentDto?> GetPayResultAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking).ThenInclude(b => b.Room)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            return payment == null ? null : ToDto(payment);
        }
    }
}
