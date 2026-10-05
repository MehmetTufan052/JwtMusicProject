using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.ArtistServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [AllowAnonymous]
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

        [HttpPost("follow/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleFollow(int id)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var following = await _artistService.ToggleFollowAsync(appUserId, id);
            return Ok(new { following });
        }

        [HttpGet("{id}/follow-status")]
        [Authorize]
        public async Task<IActionResult> GetFollowStatus(int id)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var following = await _artistService.IsFollowingAsync(appUserId, id);
            return Ok(new { following });
        }

        [HttpGet("followed")]
        [Authorize]
        public async Task<IActionResult> GetFollowed()
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var values = await _artistService.GetFollowedArtistsAsync(appUserId);
            return Ok(values);
        }
    }
}
