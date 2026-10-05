using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.SongServices
{
    public class SongService : ISongService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public SongService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddListeningHistoryAsync(string appUserId, int songId)
        {
            var history = new ListeningHistory
            {
                AppUserId = appUserId,
                SongId = songId,
                ListenDate = DateTime.Now
            };

            await _context.ListeningHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ResultSongDto>> GetAllSongsAsync()
        {
            var songs = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .Include(s => s.Package)
                .ToListAsync();

            return _mapper.Map<List<ResultSongDto>>(songs);
        }

        public async Task<Song> GetSongByIdAsync(int id)
        {
            return await _context.Songs
                .Include(s => s.Package)
                .Include(s => s.Artist)
                .FirstOrDefaultAsync(s => s.SongId == id);
        }

        public async Task<List<ResultSongDto>> GetSongsByGenreAsync(int genreId)
        {
            var songs = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .Include(s => s.Package)
                .Where(s => s.GenreId == genreId)
                .ToListAsync();

            return _mapper.Map<List<ResultSongDto>>(songs);
        }

        public async Task<List<ResultSongDto>> GetSongsByArtistAsync(int artistId)
        {
            var songs = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .Include(s => s.Package)
                .Where(s => s.ArtistId == artistId)
                .ToListAsync();

            return _mapper.Map<List<ResultSongDto>>(songs);
        }

        public async Task<ResultSongDto> GetSongDetailAsync(int id)
        {
            var song = await _context.Songs
            .Include(s => s.Artist)
            .Include(s => s.Genre)
            .Include(s => s.Package)
            .FirstOrDefaultAsync(s => s.SongId == id);

            return _mapper.Map<ResultSongDto>(song);
        }

        public async Task<List<ListeningHistoryDto>> GetHistoryAsync(string appUserId, int count = 50)
        {
            var history = await _context.ListeningHistories
                .Include(h => h.Song).ThenInclude(s => s.Artist)
                .Include(h => h.Song).ThenInclude(s => s.Package)
                .Where(h => h.AppUserId == appUserId)
                .OrderByDescending(h => h.ListenDate)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ListeningHistoryDto>>(history);
        }

        public async Task<bool> ToggleFavoriteAsync(string appUserId, int songId)
        {
            var existing = await _context.SongFavorites
                .FirstOrDefaultAsync(f => f.AppUserId == appUserId && f.SongId == songId);

            if (existing != null)
            {
                _context.SongFavorites.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }

            await _context.SongFavorites.AddAsync(new SongFavorite
            {
                AppUserId = appUserId,
                SongId = songId,
                FavoriteDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoritedAsync(string appUserId, int songId)
        {
            return await _context.SongFavorites
                .AnyAsync(f => f.AppUserId == appUserId && f.SongId == songId);
        }

        public async Task<List<ResultSongDto>> GetFavoritesAsync(string appUserId)
        {
            var songs = await _context.SongFavorites
                .Include(f => f.Song).ThenInclude(s => s.Artist)
                .Include(f => f.Song).ThenInclude(s => s.Genre)
                .Include(f => f.Song).ThenInclude(s => s.Package)
                .Where(f => f.AppUserId == appUserId)
                .OrderByDescending(f => f.FavoriteDate)
                .Select(f => f.Song)
                .ToListAsync();

            return _mapper.Map<List<ResultSongDto>>(songs);
        }

        public async Task<ResultSongDto> CreateSongAsync(CreateSongDto createSongDto)
        {
            var song = _mapper.Map<Song>(createSongDto);
            await _context.Songs.AddAsync(song);
            await _context.SaveChangesAsync();

            var created = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .Include(s => s.Package)
                .FirstAsync(s => s.SongId == song.SongId);

            return _mapper.Map<ResultSongDto>(created);
        }

        public async Task<bool> UpdateSongAsync(int id, CreateSongDto createSongDto)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.SongId == id);
            if (song == null)
            {
                return false;
            }

            song.SongName = createSongDto.SongName;
            song.CoverImageUrl = createSongDto.CoverImageUrl;
            if (!string.IsNullOrWhiteSpace(createSongDto.AudioUrl))
            {
                song.AudioUrl = createSongDto.AudioUrl;
            }
            song.Duration = createSongDto.Duration;
            song.ListenCount = createSongDto.ListenCount;
            song.ReleaseDate = createSongDto.ReleaseDate;
            song.Lyrics = createSongDto.Lyrics;
            song.ArtistId = createSongDto.ArtistId;
            song.GenreId = createSongDto.GenreId;
            song.PackageId = createSongDto.PackageId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSongAsync(int id)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.SongId == id);
            if (song == null)
            {
                return false;
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ResultSongDto>> GetRecommendationsAsync(int songId, int count = 5)
        {
            var current = await _context.Songs.FindAsync(songId);

            var songs = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .Include(s => s.Package)
                .Where(s => s.GenreId == current.GenreId && s.SongId != songId)
                .OrderByDescending(s => s.ListenCount)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ResultSongDto>>(songs);
        }
    }
}
