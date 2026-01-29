using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingManager.DataAccess.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HotelBookingContext _context;

        public AuthRepository(HotelBookingContext context)
        {
            _context = context;
        }

        // login bằng email (hoặc thêm UserName sau nếu muốn)
        public Task<User?> GetUserByLoginAsync(string login, string password)
        {
            return _context.Users.FirstOrDefaultAsync(u =>
                u.IsActive == true &&
                u.Email == login &&
                u.PasswordHash == password);
        }

        public Task<bool> IsUserNameTakenAsync(string userName)
        {
            // Nếu chưa có cột UserName thì có thể bỏ phương thức này
            return _context.Users.AnyAsync(u => u.Email == userName);
        }

        public Task<bool> IsEmailTakenAsync(string email)
        {
            return _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
