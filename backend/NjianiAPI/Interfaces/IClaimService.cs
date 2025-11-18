using NjianiAPI.Models;

public interface IClaimService
{
    Task<ClaimT?> GetClaimByIdAsync(Guid claimId);
    Task<List<ClaimT>> GetAllClaimsAsync();
    Task<ClaimT> CreateClaimAsync(ClaimCreateDto claimCreateDto);
    Task<bool> UpdateClaimAsync(Guid claimId, ClaimUpdateDto claimUpdateDto);
    Task<bool> DeleteClaimAsync(Guid claimId);
}