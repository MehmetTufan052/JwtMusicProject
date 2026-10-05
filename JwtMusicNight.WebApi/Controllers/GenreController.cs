using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.GenreServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GenreList()
        {
            var values = await _genreService.GetAllGenresAsync();
            return Ok(values);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGenre(CreateGenreDto createGenreDto)
        {
            var value = await _genreService.CreateGenreAsync(createGenreDto);
            return Ok(value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var success = await _genreService.DeleteGenreAsync(id);
            if (!success)
            {
                return NotFound("Tur bulunamadi.");
            }
            return Ok();
        }
    }
}
