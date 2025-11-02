using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLib.Data;

namespace SharedLib.Services;

public class PhotoService : IPhotoService
{
    private readonly ILogger<PhotoService> _logger;
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public PhotoService(ILogger<PhotoService> logger, IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
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
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Photos.Remove(photo);
        await dbContext.SaveChangesAsync();
        _logger.LogInformation($"Deleted photo: {photo.Id}");
    }

    public async Task<Guid> AddPhotoAsync(Photo photo)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Photos.Add(photo);
        await dbContext.SaveChangesAsync();
        
        _logger.LogInformation($"New photo added: {photo.Id}");
        return photo.Id;
    }
}