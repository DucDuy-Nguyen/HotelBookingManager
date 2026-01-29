using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}
