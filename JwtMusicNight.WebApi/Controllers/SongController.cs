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
        private readonly IWebHostEnvironment _environment;

        public SongController(ISongService songService, IWebHostEnvironment environment)
        {
            _songService = songService;
            _environment = environment;
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
                AudioUrl = $"/api/Song/stream/{song.SongId}",
                ArtistName = song.Artist.ArtistName
            });
        }

        [HttpGet("stream/{id}")]
        [Authorize]
        public async Task<IActionResult> Stream(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound("Şarkı bulunamadı.");
            }

            var packageLevelClaim = User.FindFirst("PackageLevel")?.Value;
            if (!int.TryParse(packageLevelClaim, out var userLevel))
            {
                return Forbid();
            }

            if (userLevel < song.Package.Level)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin."
                });
            }

            var musicRoot = Path.Combine(_environment.ContentRootPath, "MusicFiles");
            var safeFileName = Path.GetFileName(song.AudioUrl);
            var filePath = Path.Combine(musicRoot, safeFileName);

            if (string.IsNullOrWhiteSpace(safeFileName) || !System.IO.File.Exists(filePath))
            {
                return NotFound("MP3 dosyası bulunamadı.");
            }

            return PhysicalFile(filePath, "audio/mpeg", enableRangeProcessing: true);
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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> SongDetail(int id)
        {
            var song = await _songService.GetSongDetailAsync(id);
            if(song==null) return NotFound("Sarki bulunamadi.");
            return Ok(song);
        }

        [HttpGet("{id}/recommendations")]
        [Authorize]
        public async Task<IActionResult> GetRecommendations(int id)
        {
            var values=await _songService.GetRecommendationsAsync(id);
            return Ok(values);
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetHistory()
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var values = await _songService.GetHistoryAsync(appUserId);
            return Ok(values);
        }

        [HttpPost("favorite/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var favorited = await _songService.ToggleFavoriteAsync(appUserId, id);
            return Ok(new { favorited });
        }

        [HttpGet("{id}/favorite-status")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteStatus(int id)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var favorited = await _songService.IsFavoritedAsync(appUserId, id);
            return Ok(new { favorited });
        }

        [HttpGet("favorites")]
        [Authorize]
        public async Task<IActionResult> GetFavorites()
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var values = await _songService.GetFavoritesAsync(appUserId);
            return Ok(values);
        }
    }
}
