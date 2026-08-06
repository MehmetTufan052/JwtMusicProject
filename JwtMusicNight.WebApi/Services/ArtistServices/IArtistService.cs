using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.ArtistServices
{
    public interface IArtistService
    {
        Task<List<ResultArtistDto>> GetAllArtistsAsync();
        Task CreateArtistAsync(CreateArtistDto createArtistDto);
    }
}
