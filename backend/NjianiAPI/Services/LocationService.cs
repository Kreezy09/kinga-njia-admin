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

    public async Task<LocationResponseDto?> GetLocationByIdAsync(Guid locationId)
    {
        var location = await _context.Locations
            .Include(l => l.Claims)
            .FirstOrDefaultAsync(l => l.Id == locationId);

        if (location == null)
        {
            return null;
        }

        return new LocationResponseDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        };
    }

    public async Task<List<LocationResponseDto>> GetAllLocationsAsync()
    {
        var locations = await _context.Locations
            .Include(l => l.Claims)
            .ToListAsync();

        return locations.Select(location => new LocationResponseDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        }).ToList();
    }
}