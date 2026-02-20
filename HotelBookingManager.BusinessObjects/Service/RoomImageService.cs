using HotelBookingManager.BusinessObjects.DTO;
using HotelBookingManager.BusinessObjects.IService;
using HotelBookingManager.DataAccess.IRepositories;
using HotelBookingManager.DataAccess.Models;

public class RoomImageService : IRoomImageService
{
    private readonly IRoomImageRepository _roomImageRepository;

    public RoomImageService(IRoomImageRepository roomImageRepository)
    {
        _roomImageRepository = roomImageRepository;
    }

    // --------------------
    // MAPPING
    // --------------------
    private static RoomImageDto ToDto(RoomImage img) => new RoomImageDto
    {
        RoomImageId = img.RoomImageId,
        RoomId = img.RoomId,           // ← BÂY GIỜ OK
        ImageUrl = img.ImageUrl,
        DisplayOrder = img.DisplayOrder,
        IsThumbnail = img.IsThumbnail
    };

    // --------------------
    // 🔥 GET BY ID - FIX ✅
    // --------------------
    public async Task<RoomImageDto?> GetByIdAsync(int id)
    {
        var image = await _roomImageRepository.GetByIdAsync(id);  // Repository phải có method này
        return image == null ? null : ToDto(image);
    }

    // --------------------
    // GET LIST IMAGE
    // --------------------
    public async Task<IEnumerable<RoomImageDto>> GetByRoomIdAsync(int roomId)
    {
        var images = await _roomImageRepository.GetByRoomIdAsync(roomId);
        return images.Select(ToDto);
    }

    // --------------------
    // GET THUMBNAIL
    // --------------------
    public async Task<RoomImageDto?> GetThumbnailAsync(int roomId)
    {
        var image = await _roomImageRepository.GetThumbnailAsync(roomId);
        return image == null ? null : ToDto(image);
    }

    public async Task AddImagesAsync(int roomId, List<string> imageUrls)
    {
        var images = imageUrls.Select((url, index) => new RoomImage
        {
            RoomId = roomId,
            ImageUrl = url,
            DisplayOrder = index,
            IsThumbnail = index == 0,
            IsActive = true
        });

        await _roomImageRepository.AddRangeAsync(images);
    }

    // --------------------
    // CREATE IMAGE
    // --------------------
    public async Task<RoomImageDto> CreateAsync(int roomId, RoomImageDto dto)
    {
        var entity = new RoomImage
        {
            RoomId = roomId,
            ImageUrl = dto.ImageUrl,
            DisplayOrder = dto.DisplayOrder,
            IsThumbnail = dto.IsThumbnail,
            IsActive = true
        };

        await _roomImageRepository.AddAsync(entity);
        await _roomImageRepository.SaveChangesAsync();
        return ToDto(entity);
    }

    // --------------------
    // SET THUMBNAIL - FIX ✅
    // --------------------
    public async Task<bool> SetThumbnailAsync(int roomImageId)
    {
        var targetImage = await _roomImageRepository.GetByIdAsync(roomImageId);
        if (targetImage == null) return false;

        // 🔥 FIX: Lấy roomId từ navigation property hoặc repository
        var roomId = targetImage.Room?.RoomId ?? await GetRoomIdByImageId(roomImageId);

        // Bỏ thumbnail cũ của cùng room
        var allImages = await _roomImageRepository.GetByRoomIdAsync(roomId);
        foreach (var img in allImages.Where(i => i.IsThumbnail))
        {
            img.IsThumbnail = false;
            _roomImageRepository.Update(img);
        }

        targetImage.IsThumbnail = true;
        _roomImageRepository.Update(targetImage);
        await _roomImageRepository.SaveChangesAsync();
        return true;
    }

    // 🔥 Helper method
    private async Task<int> GetRoomIdByImageId(int imageId)
    {
        var image = await _roomImageRepository.GetByIdAsync(imageId);
        return image?.Room?.RoomId ?? 0;
    }


    // --------------------
    // DELETE (SOFT) - FIX ✅
    // --------------------
    public async Task<bool> DeleteAsync(int roomImageId)
    {
        var image = await _roomImageRepository.GetByIdAsync(roomImageId);
        if (image == null) return false;

        image.IsActive = false;
        _roomImageRepository.Update(image);
        await _roomImageRepository.SaveChangesAsync();
        return true;
    }
}
