public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string? publicId = null);
    Task<bool> DeleteImageAsync(string publicId);
}