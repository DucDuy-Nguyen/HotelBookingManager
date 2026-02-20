using HotelBookingManager.BusinessObjects.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingManager.BusinessObjects.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(UserDto user);
        Task<bool> DeleteAsync(int id); // xoá mềm
    }

}
