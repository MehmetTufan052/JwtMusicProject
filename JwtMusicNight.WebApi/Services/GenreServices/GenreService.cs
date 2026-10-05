using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.GenreServices
{
    public class GenreService : IGenreService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public GenreService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ResultGenreDto>> GetAllGenresAsync()
        {
            var values = await _context.Genres.ToListAsync();
            return _mapper.Map<List<ResultGenreDto>>(values);
        }

        public async Task<ResultGenreDto> CreateGenreAsync(CreateGenreDto createGenreDto)
        {
            var genre = _mapper.Map<Genre>(createGenreDto);
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
            return _mapper.Map<ResultGenreDto>(genre);
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.GenreId == id);
            if (genre == null)
            {
                return false;
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
