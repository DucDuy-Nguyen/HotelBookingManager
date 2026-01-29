using HotelBookingManager.DataAccess.Models;

namespace HotelBookingManager.DataAccess.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByLoginAsync(string login, string password);
        Task<bool> IsUserNameTakenAsync(string userName);   // dùng sau nếu có UserName
        Task<bool> IsEmailTakenAsync(string email);
        Task AddUserAsync(User user);

        Task<User?> GetByEmailAsync(string email);
        Task UpdateAsync(User user);
    }
}
