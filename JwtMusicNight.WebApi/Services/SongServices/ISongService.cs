using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;

namespace JwtMusicNight.WebApi.Services.SongServices
{
    public interface ISongService
    {
        Task<Song> GetSongByIdAsync(int id);
        Task AddListeningHistoryAsync(string appUserId, int songId);
        Task<List<ResultSongDto>> GetAllSongsAsync();
        Task<List<ResultSongDto>> GetSongsByGenreAsync(int genreId);
        Task<List<ResultSongDto>> GetSongsByArtistAsync(int artistId);
        Task<ResultSongDto> GetSongDetailAsync(int id);
        Task<List<ResultSongDto>> GetRecommendationsAsync(int songId, int count = 5);
    }
}
