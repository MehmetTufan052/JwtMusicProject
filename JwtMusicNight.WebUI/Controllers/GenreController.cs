using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace JwtMusicNight.WebUI.Controllers
{
    public class GenreController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GenreController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> GenreList()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("SignIn", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var genreResponse = await client.GetAsync("https://localhost:7292/api/Genre");
            var songResponse = await client.GetAsync("https://localhost:7292/api/Song");

            if (!genreResponse.IsSuccessStatusCode || !songResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("SignIn", "Login");
            }

            var genresJson = await genreResponse.Content.ReadAsStringAsync();
            var songsJson = await songResponse.Content.ReadAsStringAsync();

            var model = new GenrePageViewModel
            {
                Genres = JsonConvert.DeserializeObject<List<ResultGenreDto>>(genresJson) ?? new(),
                Songs = JsonConvert.DeserializeObject<List<ResultSongDto>>(songsJson) ?? new(),
                UserPackageName = HttpContext.Session.GetString("PackageName") ?? "Basic",
                UserPackageLevel = HttpContext.Session.GetInt32("PackageLevel") ?? 0
            };

            return View(model);
        }
    }
}
