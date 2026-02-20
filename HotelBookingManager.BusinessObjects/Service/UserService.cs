using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IHotelRepository _hotelRepo;

        public UserService(IUserRepository userRepo, IHotelRepository hotelRepo)
        {
            _userRepo = userRepo;
            _hotelRepo = hotelRepo;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();
            var hotels = await _hotelRepo.GetAllAsync();

            var hotelDict = hotels.ToDictionary(h => h.HotelId, h => h.Name);

            return users.Select(u => new UserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                HotelId = u.HotelId,
                HotelName = (u.HotelId.HasValue && hotelDict.ContainsKey(u.HotelId.Value))
                            ? hotelDict[u.HotelId.Value]
                            : null,
                IsActive = u.IsActive
            });
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _userRepo.GetByIdAsync(id);
            if (u == null) return null;

            // Không cần HotelName khi Edit/Delete, có cũng được
            return new UserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                HotelId = u.HotelId,
                IsActive = u.IsActive
            };
        }

        public async Task<bool> UpdateAsync(UserDto dto)
        {
            var u = await _userRepo.GetByIdAsync(dto.UserId);
            if (u == null) return false;

            u.FullName = dto.FullName;
            u.Email = dto.Email;
            u.PhoneNumber = dto.PhoneNumber;
            u.RoleId = dto.RoleId;
            u.HotelId = dto.HotelId;
            u.IsActive = dto.IsActive;

            await _userRepo.UpdateAsync(u);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var u = await _userRepo.GetByIdAsync(id);
            if (u == null) return false;

            u.IsActive = false;
            await _userRepo.UpdateAsync(u);
            return true;
        }
    }
}
