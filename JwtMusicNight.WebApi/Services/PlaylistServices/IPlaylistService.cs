using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.PlaylistServices
{
    public interface IPlaylistService
    {
        Task<ResultPlaylistDto> CreatePlaylistAsync(string appUserId, CreatePlaylistDto createPlaylistDto);
        Task<List<ResultPlaylistDto>> GetMyPlaylistsAsync(string appUserId);
        Task<PlaylistDetailDto> GetPlaylistDetailAsync(string appUserId, int playlistId);
        Task<bool> AddSongAsync(string appUserId, int playlistId, int songId);
        Task<bool> RemoveSongAsync(string appUserId, int playlistId, int songId);
        Task<bool> DeletePlaylistAsync(string appUserId, int playlistId);
    }
}
