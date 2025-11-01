using Microsoft.AspNetCore.Components.Forms;

namespace SharedLib.Services;

public interface IPhotoService
{
    Task<List<Photo>> GetPhotosAsync(int blogPostId);
    Task UpdatePhotoAsync(Photo photo);
    Task DeletePhotoAsync(Photo photo);
    
    Task<Guid> AddPhotoAsync(Photo photo);
}