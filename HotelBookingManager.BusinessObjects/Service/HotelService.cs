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
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        // ====== MAPPING ======

        private static HotelDto ToDto(Hotel h) => new HotelDto
        {
            HotelId = h.HotelId,
            Name = h.Name,
            Description = h.Description,
            Address = h.Address,
            City = h.City,
            Country = h.Country,
            Rating = (double?)h.Rating,
            PhoneNumber = h.PhoneNumber,
            Email = h.Email,
            IsActive = h.IsActive,
            ImageUrl = h.ImageUrl           // thêm

        };

        private static Hotel ToEntity(HotelDto dto) => new Hotel
        {
            HotelId = dto.HotelId,
            Name = dto.Name,
            Description = dto.Description,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            Rating = (decimal?)dto.Rating,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            IsActive = dto.IsActive,
            ImageUrl = dto.ImageUrl         // thêm

        };

        // ====== IMPLEMENT IHotelService với DTO ======

        public async Task<IEnumerable<HotelDto>> GetAllAsync()
        {
            var hotels = await _hotelRepository.GetAllAsync();
            return hotels.Select(ToDto);
        }

        public async Task<HotelDto?> GetByIdAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            return hotel == null ? null : ToDto(hotel);
        }

        public async Task<HotelDto> CreateAsync(HotelDto dto)
        {
            var hotel = ToEntity(dto);
            hotel.CreatedAt = DateTime.UtcNow;
            hotel.IsActive = true;

            await _hotelRepository.AddAsync(hotel);
            await _hotelRepository.SaveChangesAsync();

            return ToDto(hotel);
        }

        public async Task<bool> UpdateAsync(HotelDto dto)
        {
            var existing = await _hotelRepository.GetByIdAsync(dto.HotelId);
            if (existing == null) return false;

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Address = dto.Address;
            existing.City = dto.City;
            existing.Country = dto.Country;
            existing.Rating = (decimal?)dto.Rating;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Email = dto.Email;
            existing.IsActive = dto.IsActive;
            existing.ImageUrl = dto.ImageUrl;


            _hotelRepository.Update(existing);
            await _hotelRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);
            if (hotel == null) return false;

            hotel.IsActive = false;

            _hotelRepository.Update(hotel);
            await _hotelRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<HotelDto>> GetFilteredAsync(string? city, string statusFilter)
        {
            var hotels = await _hotelRepository.GetAllAsync(); // entity

            if (!string.IsNullOrWhiteSpace(city))
            {
                var c = city.Trim();
                hotels = hotels.Where(h =>
                    h.City.Contains(c, StringComparison.OrdinalIgnoreCase));
            }

            statusFilter = (statusFilter ?? "active").ToLowerInvariant();

            hotels = statusFilter switch
            {
                "active" => hotels.Where(h => h.IsActive),
                "inactive" => hotels.Where(h => !h.IsActive),
                _ => hotels
            };

            return hotels.Select(ToDto);
        }

        public async Task<IEnumerable<HotelDto>> SearchByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return Array.Empty<HotelDto>();

            var hotels = await _hotelRepository.SearchByCityAsync(city);
            return hotels.Select(ToDto);
        }
    }
}