// Presentation/Models/FeedbackCreateViewModel.cs
using HotelBookingManager.BusinessObjects.DTO;

namespace HotelBookingManager.Presentation.Models
{
    public class FeedbackCreateViewModel
    {
        public FeedbackDto Feedback { get; set; } = new FeedbackDto();

        public IEnumerable<HotelDto> Hotels { get; set; }
            = Enumerable.Empty<HotelDto>();

        public IEnumerable<BookingDto> Bookings { get; set; }
            = Enumerable.Empty<BookingDto>();
    }
}
