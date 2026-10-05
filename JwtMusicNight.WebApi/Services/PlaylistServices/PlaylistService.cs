using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.PlaylistServices
{
    public class PlaylistService : IPlaylistService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public PlaylistService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResultPlaylistDto> CreatePlaylistAsync(string appUserId, CreatePlaylistDto createPlaylistDto)
        {
            var playlist = new Playlist
            {
                Name = createPlaylistDto.Name,
                AppUserId = appUserId,
                CreatedDate = DateTime.Now,
                PlaylistSongs = new List<PlaylistSong>()
            };

            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();

            return _mapper.Map<ResultPlaylistDto>(playlist);
        }

        public async Task<List<ResultPlaylistDto>> GetMyPlaylistsAsync(string appUserId)
        {
            var playlists = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .Where(p => p.AppUserId == appUserId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return _mapper.Map<List<ResultPlaylistDto>>(playlists);
        }

        public async Task<PlaylistDetailDto> GetPlaylistDetailAsync(string appUserId, int playlistId)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song).ThenInclude(s => s.Artist)
                .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song).ThenInclude(s => s.Genre)
                .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song).ThenInclude(s => s.Package)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.AppUserId == appUserId);

            return playlist == null ? null : _mapper.Map<PlaylistDetailDto>(playlist);
        }

        public async Task<bool> AddSongAsync(string appUserId, int playlistId, int songId)
        {
            var playlistExists = await _context.Playlists
                .AnyAsync(p => p.PlaylistId == playlistId && p.AppUserId == appUserId);

            if (!playlistExists)
            {
                return false;
            }

            var alreadyAdded = await _context.PlaylistSongs
                .AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (alreadyAdded)
            {
                return true;
            }

            await _context.PlaylistSongs.AddAsync(new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId,
                AddedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSongAsync(string appUserId, int playlistId, int songId)
        {
            var playlistSong = await _context.PlaylistSongs
                .Include(ps => ps.Playlist)
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId && ps.Playlist.AppUserId == appUserId);

            if (playlistSong == null)
            {
                return false;
            }

            _context.PlaylistSongs.Remove(playlistSong);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePlaylistAsync(string appUserId, int playlistId)
        {
            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.AppUserId == appUserId);

            if (playlist == null)
            {
                return false;
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
