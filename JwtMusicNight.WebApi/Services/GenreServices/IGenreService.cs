using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.GenreServices
{
    public interface IGenreService
    {
        Task<List<ResultGenreDto>> GetAllGenresAsync();
        Task<ResultGenreDto> CreateGenreAsync(CreateGenreDto createGenreDto);
        Task<bool> DeleteGenreAsync(int id);
    }
}
