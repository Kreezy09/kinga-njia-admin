using Microsoft.EntityFrameworkCore;
using NjianiAPI.Data;

public class LocationService : ILocationService
{
    private readonly NjianiDbContext _context;

    public LocationService(NjianiDbContext context)
    {
        _context = context;
    }

    public async Task<LocationResponseDto> CreateLocationAsync(LocationCreateDto locationCreateDto)
    {
        var location = new Location
        {
            Name = locationCreateDto.Name,
            Latitude = locationCreateDto.Latitude,
            Longitude = locationCreateDto.Longitude,
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return new LocationResponseDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        };
    }
}