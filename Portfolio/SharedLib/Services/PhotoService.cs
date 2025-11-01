using Microsoft.Extensions.Logging;
using SharedLib.Data;

namespace SharedLib.Services;

public class PhotoService : IPhotoService
{
    private readonly ILogger<PhotoService> _logger;
    private readonly AppDbContext _context;

    public PhotoService(ILogger<PhotoService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public Task<List<Photo>> GetPhotosAsync(int blogPostId)
    {
        throw new NotImplementedException();
    }

    public Task UpdatePhotoAsync(Photo photo)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePhotoAsync(Photo photo)
    {
        _context.Photos.Remove(photo);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Deleted photo: {photo.Id}");
    }

    public async Task<Guid> AddPhotoAsync(Photo photo)
    {
        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"New photo added: {photo.Id}");
        return photo.Id;
    }
}