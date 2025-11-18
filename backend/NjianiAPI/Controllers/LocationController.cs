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
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocationById(Guid id)
    {
        var location = await _locationService.GetLocationByIdAsync(id);
        if (location == null)
        {
            return NotFound();
        }
        return Ok(location);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllLocations()
    {
        var locations = await _locationService.GetAllLocationsAsync();
        return Ok(locations);
    }
}