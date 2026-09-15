using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
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
    }
}
