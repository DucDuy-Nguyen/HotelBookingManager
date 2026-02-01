using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingManager.BusinessObjects.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        // MAPPING
        private static RoomDto ToDto(Room r) => new RoomDto
        {
            RoomId = r.RoomId,
            HotelId = r.HotelId,
            RoomTypeId = r.RoomTypeId,
            HotelName = r.Hotel?.Name,          // an toàn
            RoomTypeName = r.RoomType?.Name,   // <-- GÁN TÊN LOẠI PHÒNG

            RoomNumber = r.RoomNumber,
            Floor = r.Floor,
            Status = r.Status,
            CurrentPrice = r.CurrentPrice,
            IsActive = r.IsActive
        };

        private static Room ToEntity(RoomDto dto) => new Room
        {
            RoomId = dto.RoomId,
            HotelId = dto.HotelId,
            RoomTypeId = dto.RoomTypeId,
            RoomNumber = dto.RoomNumber,
            Floor = dto.Floor,
            Status = dto.Status,
            CurrentPrice = dto.CurrentPrice,
            IsActive = dto.IsActive
        };

        public async Task<IEnumerable<RoomDto>> GetByHotelAsync(int hotelId, bool onlyAvailable)
        {
            var rooms = await _roomRepository.GetByHotelAsync(hotelId, onlyAvailable);
            return rooms.Select(ToDto);
        }

        public async Task<RoomDto?> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return room == null ? null : ToDto(room);
        }

        public async Task<IEnumerable<RoomDto>> GetAllAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return rooms.Select(ToDto);
        }

        public async Task<RoomDto> CreateAsync(RoomDto dto)
        {
            var room = ToEntity(dto);
            room.IsActive = true;

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            return ToDto(room);
        }

        public async Task<bool> UpdateAsync(RoomDto dto)
        {
            var existing = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (existing == null) return false;

            existing.HotelId = dto.HotelId;
            existing.RoomTypeId = dto.RoomTypeId;
            existing.RoomNumber = dto.RoomNumber;
            existing.Floor = dto.Floor;
            existing.Status = dto.Status;
            existing.CurrentPrice = dto.CurrentPrice;
            existing.IsActive = dto.IsActive;

            _roomRepository.Update(existing);
            await _roomRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return false;

            room.IsActive = false; // soft delete
            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoomDto>> GetByStatusAsync(string status)
        {
            var rooms = await _roomRepository.GetByStatusAsync(status);
            return rooms.Select(ToDto);
        }
    }
}
