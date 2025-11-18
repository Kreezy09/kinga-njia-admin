public interface ILocationService
{
    Task<LocationResponseDto> CreateLocationAsync(LocationCreateDto locationCreateDto);
    Task<LocationResponseDto?> GetLocationByIdAsync(Guid locationId);
    Task<List<LocationResponseDto>> GetAllLocationsAsync();
}