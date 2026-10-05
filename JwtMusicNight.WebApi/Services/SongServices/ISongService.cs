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
        Task<List<ListeningHistoryDto>> GetHistoryAsync(string appUserId, int count = 50);
        Task<bool> ToggleFavoriteAsync(string appUserId, int songId);
        Task<bool> IsFavoritedAsync(string appUserId, int songId);
        Task<List<ResultSongDto>> GetFavoritesAsync(string appUserId);
        Task<ResultSongDto> CreateSongAsync(CreateSongDto createSongDto);
        Task<bool> UpdateSongAsync(int id, CreateSongDto createSongDto);
        Task<bool> DeleteSongAsync(int id);
    }
}
