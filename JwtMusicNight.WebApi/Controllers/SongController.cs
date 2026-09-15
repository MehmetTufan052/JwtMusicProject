using JwtMusicNight.WebApi.Services.SongServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpPost("listen/{id}")]
        [Authorize]
        public async Task<IActionResult> Listen(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);

            if (song == null)
            {
                return NotFound("Sarki bulunamadi.");
            }

            var userLevel = int.Parse(User.FindFirst("PackageLevel").Value);

            if (userLevel < song.Package.Level)
            {
                return StatusCode(403, new
                {
                    message = "Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin."
                });
            }

            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _songService.AddListeningHistoryAsync(appUserId, id);

            return Ok(new
            {
                song.SongId,
                song.SongName,
                song.AudioUrl,
                ArtistName = song.Artist.ArtistName
            });
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SongList()
        {
            var values = await _songService.GetAllSongsAsync();
            return Ok(values);
        }

        [HttpGet("bygenre/{genreId}")]
        [Authorize]
        public async Task<IActionResult> SongsByGenre(int genreId)
        {
            var values = await _songService.GetSongsByGenreAsync(genreId);
            return Ok(values);
        }

        [HttpGet("byartist/{artistId}")]
        [Authorize]
        public async Task<IActionResult> SongsByArtist(int artistId)
        {
            var values = await _songService.GetSongsByArtistAsync(artistId);
            return Ok(values);
        }
    }
}
