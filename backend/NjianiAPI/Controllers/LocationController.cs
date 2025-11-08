using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] LocationCreateDto locationCreateDto)
    {
        var createdLocation = await _locationService.CreateLocationAsync(locationCreateDto);
        return CreatedAtAction(nameof(CreateLocation), new { id = createdLocation.Id }, createdLocation);
    }
}