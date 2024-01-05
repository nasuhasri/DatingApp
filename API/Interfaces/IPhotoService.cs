using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    // handle photo upload and photo deletion
    public interface IPhotoService
    {
        // IFormFile: Represents a file sent with the HttpRequest
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}