using System.Security.Cryptography;
using System.Text;
using System.Linq;

public static class ClaimHasher
{
    public static string GenerateClaimHash(ClaimT claim)
    {
        // Sort image URLs (or hashes) so order doesn’t affect result
        var imageParts = claim.Images
            .OrderBy(i => i.ImageUrl)
            .Select(i => i.ImageUrl)
            .ToArray();

        // Build input string
        var input = string.Join("|", new[]
        {
            string.Join(",", imageParts),
            claim.CreatedAt.ToString("O"),  // ISO 8601
            claim.Location?.Latitude.ToString() ?? "",
            claim.Location?.Longitude.ToString() ?? ""
        });

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes); // uppercase hex string
    }
}
