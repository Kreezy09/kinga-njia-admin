public interface ILocationService
{
    Task<LocationResponseDto> CreateLocationAsync(LocationCreateDto locationCreateDto);
}