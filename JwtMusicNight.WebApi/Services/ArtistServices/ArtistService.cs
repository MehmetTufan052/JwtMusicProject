using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.ArtistServices
{
    public class ArtistService : IArtistService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public ArtistService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateArtistAsync(CreateArtistDto createArtistDto)
        {
            var value = _mapper.Map<Artist>(createArtistDto);
            await _context.Artists.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ResultArtistDto>> GetAllArtistsAsync()
        {
            var values = await _context.Artists.ToListAsync();
            return _mapper.Map<List<ResultArtistDto>>(values);
        }

        public async Task<ResultArtistDto> GetArtistByIdAsync(int id)
        {
            var value = await _context.Artists.FirstOrDefaultAsync(x => x.ArtistId == id);
            return _mapper.Map<ResultArtistDto>(value);
        }
    }
}
