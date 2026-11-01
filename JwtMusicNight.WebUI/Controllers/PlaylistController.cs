using JwtMusicNight.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace JwtMusicNight.WebUI.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PlaylistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient AuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JwtToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("SignIn", "Login");

            var client = AuthorizedClient();
            var response = await client.GetAsync("http://localhost:5253/api/Playlist");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("SignIn", "Login");

            var json = await response.Content.ReadAsStringAsync();
            var playlists = JsonConvert.DeserializeObject<List<ResultPlaylistDto>>(json) ?? new();

            ViewBag.PackageName = HttpContext.Session.GetString("PackageName") ?? "Basic";
            ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            return View(playlists);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("SignIn", "Login");

            var client = AuthorizedClient();
            var response = await client.GetAsync($"http://localhost:5253/api/Playlist/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var playlist = JsonConvert.DeserializeObject<PlaylistDetailDto>(json);

            var packageLevel = HttpContext.Session.GetInt32("PackageLevel") ?? 0;
            var songsResponse = await client.GetAsync("http://localhost:5253/api/Song");
            var availableSongs = new List<ResultSongDto>();
            if (songsResponse.IsSuccessStatusCode)
            {
                var songsJson = await songsResponse.Content.ReadAsStringAsync();
                var allSongs = JsonConvert.DeserializeObject<List<ResultSongDto>>(songsJson) ?? new();
                var playlistSongIds = playlist?.Songs?.Select(x => x.SongId).ToHashSet() ?? new HashSet<int>();
                availableSongs = allSongs
                    .Where(x => x.PackageLevel <= packageLevel && !playlistSongIds.Contains(x.SongId))
                    .OrderByDescending(x => x.ListenCount)
                    .ThenBy(x => x.SongName)
                    .ToList();
            }

            ViewBag.PackageName = HttpContext.Session.GetString("PackageName") ?? "Basic";
            ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "True";
            ViewBag.PackageLevel = packageLevel;
            ViewBag.AvailableSongs = availableSongs;

            return View(playlist);
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var client = AuthorizedClient();
            var response = await client.GetAsync("http://localhost:5253/api/Playlist");
            var body = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = body,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var client = AuthorizedClient();
            var jsonData = JsonConvert.SerializeObject(new CreatePlaylistDto { Name = name });
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:5253/api/Playlist", content);
            var body = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = body,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpPost]
        public async Task<IActionResult> AddSong(int playlistId, int songId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var client = AuthorizedClient();
            var response = await client.PostAsync($"http://localhost:5253/api/Playlist/{playlistId}/songs/{songId}", null);

            return StatusCode((int)response.StatusCode);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSong(int playlistId, int songId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var client = AuthorizedClient();
            var response = await client.DeleteAsync($"http://localhost:5253/api/Playlist/{playlistId}/songs/{songId}");

            return StatusCode((int)response.StatusCode);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("SignIn", "Login");

            var client = AuthorizedClient();
            await client.DeleteAsync($"http://localhost:5253/api/Playlist/{id}");

            return RedirectToAction("Index");
        }
    }
}
