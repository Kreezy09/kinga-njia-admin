public interface IClaimService
{
    Task<ClaimResponseDto?> GetClaimByIdAsync(Guid claimId);
    Task<List<ClaimResponseDto>> GetAllClaimsAsync();
    Task<ClaimResponseDto> CreateClaimAsync(ClaimCreateDto claimCreateDto, Guid userId);
    Task<bool> UpdateClaimAsync(Guid claimId, ClaimUpdateDto claimUpdateDto);
    Task<bool> DeleteClaimAsync(Guid claimId);
}