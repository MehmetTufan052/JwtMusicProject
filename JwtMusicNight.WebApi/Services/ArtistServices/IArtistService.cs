using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.ArtistServices
{
    public interface IArtistService
    {
        Task<List<ResultArtistDto>> GetAllArtistsAsync();
        Task CreateArtistAsync(CreateArtistDto createArtistDto);

        Task<ResultArtistDto> GetArtistByIdAsync(int id);
        Task<bool> ToggleFollowAsync(string appUserId, int artistId);
        Task<bool> IsFollowingAsync(string appUserId, int artistId);
        Task<List<ResultArtistDto>> GetFollowedArtistsAsync(string appUserId);
    }
}
