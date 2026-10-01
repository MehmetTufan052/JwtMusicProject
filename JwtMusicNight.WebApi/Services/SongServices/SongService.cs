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
