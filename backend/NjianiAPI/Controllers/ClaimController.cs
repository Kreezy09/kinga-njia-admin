using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClaimController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimController(IClaimService claimService)
    {
        _claimService = claimService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClaimById(Guid id)
    {
        var claim = await _claimService.GetClaimByIdAsync(id);
        if (claim == null)
        {
            return NotFound();
        }

        return Ok(claim);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClaims()
    {
        var claims = await _claimService.GetAllClaimsAsync();
        return Ok(claims);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClaim([FromForm] ClaimCreateDto claimCreateDto)
    {
        // Get userId from authenticated user's claims
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"Creating claim for userId: {userId}");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        // claimCreateDto.UserId = Guid.Parse(userId);
        var createdClaim = await _claimService.CreateClaimAsync(claimCreateDto, Guid.Parse(userId));
        return CreatedAtAction(nameof(GetClaimById), new { id = createdClaim.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClaim(Guid id, [FromBody] ClaimUpdateDto
    claimUpdateDto)
    {
        var updatedClaim = await _claimService.UpdateClaimAsync(id, claimUpdateDto);
        if (updatedClaim == null)
        {
            return NotFound();
        }
        return Ok(updatedClaim);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClaim(Guid id)
    {
        var success = await _claimService.DeleteClaimAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}