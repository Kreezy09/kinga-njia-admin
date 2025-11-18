using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

public class CloudinarySettings
{
    public string CloudName { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string ApiSecret { get; set; } = "";
}

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
    {
        var settings = cloudinarySettings.Value;
        
        Account account = new Account(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret
        );
        
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file, string? publicId = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed.");

        // Validate file size (max 10MB)
        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("File size cannot exceed 10MB");

        using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = publicId,
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto"),
            Folder = "claim-images"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        
        if (uploadResult.Error != null)
            throw new Exception($"Image upload failed: {uploadResult.Error.Message}");

        return uploadResult.SecureUrl.ToString();
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }
}