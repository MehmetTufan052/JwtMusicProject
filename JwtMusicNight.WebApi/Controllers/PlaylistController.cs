using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.PlaylistServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPlaylists()
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var values = await _playlistService.GetMyPlaylistsAsync(appUserId);
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist(CreatePlaylistDto createPlaylistDto)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var value = await _playlistService.CreatePlaylistAsync(appUserId, createPlaylistDto);
            return Ok(value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistDetail(int id)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var value = await _playlistService.GetPlaylistDetailAsync(appUserId, id);

            if (value == null)
            {
                return NotFound("Çalma listesi bulunamadı.");
            }

            return Ok(value);
        }

        [HttpPost("{id}/songs/{songId}")]
        public async Task<IActionResult> AddSong(int id, int songId)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var success = await _playlistService.AddSongAsync(appUserId, id, songId);

            if (!success)
            {
                return NotFound("Çalma listesi bulunamadı.");
            }

            return Ok();
        }

        [HttpDelete("{id}/songs/{songId}")]
        public async Task<IActionResult> RemoveSong(int id, int songId)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var success = await _playlistService.RemoveSongAsync(appUserId, id, songId);

            if (!success)
            {
                return NotFound("Kayıt bulunamadı.");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var appUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var success = await _playlistService.DeletePlaylistAsync(appUserId, id);

            if (!success)
            {
                return NotFound("Çalma listesi bulunamadı.");
            }

            return Ok();
        }
    }
}
