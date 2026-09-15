using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.GenreServices
{
    public interface IGenreService
    {
        Task<List<ResultGenreDto>> GetAllGenresAsync();
    }
}
