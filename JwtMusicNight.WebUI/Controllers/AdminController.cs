using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace JwtMusicNight.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
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

        private IActionResult GuardOrNull()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            {
                return RedirectToAction("SignIn", "Login");
            }
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                return RedirectToAction("Index", "Home");
            }
            return null;
        }

        private async Task<T> GetJson<T>(string url)
        {
            var client = AuthorizedClient();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) { return default; }
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public IActionResult Index()
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }
            return View();
        }

        // ---------- SONGS ----------

        public async Task<IActionResult> Songs(int? editId)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var songs = await GetJson<List<ResultSongDto>>("https://localhost:7292/api/Song") ?? new();
            var artists = await GetJson<List<ResultArtistDto>>("https://localhost:7292/api/Artist") ?? new();
            var genres = await GetJson<List<ResultGenreDto>>("https://localhost:7292/api/Genre") ?? new();
            var packages = await GetJson<List<ResultPackageDto>>("https://localhost:7292/api/Admin/packages") ?? new();

            var model = new AdminSongsViewModel
            {
                Songs = songs,
                Artists = artists,
                Genres = genres,
                Packages = packages,
                EditingSong = editId.HasValue ? songs.FirstOrDefault(s => s.SongId == editId.Value) : null
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSong(CreateSongDto dto)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            await client.PostAsync("https://localhost:7292/api/Song", content);

            return RedirectToAction("Songs");
        }

        [HttpPost]
        public async Task<IActionResult> EditSong(int id, CreateSongDto dto)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            await client.PutAsync($"https://localhost:7292/api/Song/{id}", content);

            return RedirectToAction("Songs");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            await client.DeleteAsync($"https://localhost:7292/api/Song/{id}");

            return RedirectToAction("Songs");
        }

        // ---------- ARTISTS ----------

        public async Task<IActionResult> Artists(int? editId)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var artists = await GetJson<List<ResultArtistDto>>("https://localhost:7292/api/Artist") ?? new();
            ViewBag.EditingArtist = editId.HasValue ? artists.FirstOrDefault(a => a.ArtistId == editId.Value) : null;
            return View(artists);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArtist(CreateArtistDto dto)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            dto.CreatedDate = DateTime.Now;
            var client = AuthorizedClient();
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            await client.PostAsync("https://localhost:7292/api/Artist", content);

            return RedirectToAction("Artists");
        }

        [HttpPost]
        public async Task<IActionResult> EditArtist(int id, CreateArtistDto dto)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            await client.PutAsync($"https://localhost:7292/api/Artist/{id}", content);

            return RedirectToAction("Artists");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            await client.DeleteAsync($"https://localhost:7292/api/Artist/{id}");

            return RedirectToAction("Artists");
        }

        // ---------- GENRES ----------

        public async Task<IActionResult> Genres()
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var genres = await GetJson<List<ResultGenreDto>>("https://localhost:7292/api/Genre") ?? new();
            return View(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(string genreName)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            var content = new StringContent(JsonConvert.SerializeObject(new CreateGenreDto { GenreName = genreName }), Encoding.UTF8, "application/json");
            await client.PostAsync("https://localhost:7292/api/Genre", content);

            return RedirectToAction("Genres");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            await client.DeleteAsync($"https://localhost:7292/api/Genre/{id}");

            return RedirectToAction("Genres");
        }

        // ---------- USERS ----------

        public async Task<IActionResult> Users()
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var model = new AdminUsersViewModel
            {
                Users = await GetJson<List<UserSummaryDto>>("https://localhost:7292/api/Admin/users") ?? new(),
                Packages = await GetJson<List<ResultPackageDto>>("https://localhost:7292/api/Admin/packages") ?? new()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPackage(string id, int packageId)
        {
            var guard = GuardOrNull();
            if (guard != null) { return guard; }

            var client = AuthorizedClient();
            await client.PutAsync($"https://localhost:7292/api/Admin/users/{id}/package?packageId={packageId}", null);

            return RedirectToAction("Users");
        }
    }
}
