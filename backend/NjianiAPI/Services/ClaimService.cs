using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NjianiAPI.Data;
using NjianiAPI.Models;

public class ClaimService : IClaimService
{
    private readonly NjianiDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public ClaimService(NjianiDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ClaimT?> GetClaimByIdAsync(Guid claimId)
    {
        return await _context.Claims
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == claimId);
    }

    public async Task<List<ClaimT>> GetAllClaimsAsync()
    {
        return await _context.Claims
            .Include(c => c.Images)
            .ToListAsync();
    }

    public async Task<ClaimT> CreateClaimAsync(ClaimCreateDto claimCreateDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {

            var claim = new ClaimT
            {
                Type = claimCreateDto.Type,
                Description = claimCreateDto.Description,
                Severity = claimCreateDto.Severity,
                Comment = claimCreateDto.Comment,
                LocationId = claimCreateDto.LocationId,
                UserId = claimCreateDto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            // Upload images to Cloudinary and create claim images
            if (claimCreateDto.Images != null && claimCreateDto.Images.Any())
            {
                var claimImages = new List<ClaimImage>();

                foreach (var imageFile in claimCreateDto.Images)
                {
                    // Upload to Cloudinary
                    var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile);

                    var claimImage = new ClaimImage
                    {
                        ClaimId = claim.Id,
                        ImageUrl = imageUrl,
                        Caption = null,
                        UploadedAt = DateTime.UtcNow
                    };

                    claimImages.Add(claimImage);
                }

                _context.ClaimImages.AddRange(claimImages);
                await _context.SaveChangesAsync();
            }

            // Reload claim with images to generate hash
            var claimWithImages = await _context.Claims
                .Include(c => c.Images)
                .FirstAsync(c => c.Id == claim.Id);

            // Generate hash value after images are saved
            var hashValue = ClaimHasher.GenerateClaimHash(claimWithImages);
            claimWithImages.HashValue = hashValue;
            
            _context.Claims.Update(claimWithImages);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return claimWithImages;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateClaimAsync(Guid claimId, ClaimUpdateDto claimUpdateDto)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim == null) return false;

        claim.Type = claimUpdateDto.Type;
        claim.Description = claimUpdateDto.Description;
        claim.LocationId = claimUpdateDto.LocationId;
        claim.UserId = claimUpdateDto.UserId;
        claim.UpdatedAt = DateTime.UtcNow;
        _context.Claims.Update(claim);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteClaimAsync(Guid claimId)
    {
        var claim = await _context.Claims.FindAsync(claimId);
        if (claim == null) return false;

        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();

        return true;
    }
}