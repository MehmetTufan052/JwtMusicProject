using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.ArtistServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistController(IArtistService artistService)
        {
            _artistService = artistService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ArtistList()
        {
            var values = await _artistService.GetAllArtistsAsync();
            return Ok(values);
        }
         
        [HttpPost]
        public async Task<IActionResult> CreateArtist(CreateArtistDto createArtistDto)
        {
            await _artistService.CreateArtistAsync(createArtistDto);
            return Ok("Başarılı!");
        }
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetArtist(int id)
        {
            var value = await _artistService.GetArtistByIdAsync(id);

            if (value == null)
            {
                return NotFound("Sanatci bulunamadi.");
            }

            return Ok(value);
        }
    }
}
