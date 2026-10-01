using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace JwtMusicNight.WebUI.Controllers
{
    public class SongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SongController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Charts()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("SignIn", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7292/api/Song");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("SignIn", "Login");
            }

            var json = await response.Content.ReadAsStringAsync();
            var songs = JsonConvert.DeserializeObject<List<ResultSongDto>>(json);

            ViewBag.PackageName = HttpContext.Session.GetString("PackageName") ?? "Basic";
            ViewBag.PackageLevel = HttpContext.Session.GetInt32("PackageLevel") ?? 0;

            return View(songs);
        }

        [HttpPost]
        public async Task<IActionResult> Listen(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync("https://localhost:7292/api/Song/listen/" + id, null);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = Newtonsoft.Json.Linq.JObject.Parse(body);
                json["audioUrl"] = Url.Action(nameof(Stream), "Song", new { id });
                body = json.ToString(Newtonsoft.Json.Formatting.None);
            }

            return new ContentResult
            {
                Content = body,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpGet]
        public async Task<IActionResult> Stream(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized();
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7292/api/Song/stream/" + id);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var audioBytes = await response.Content.ReadAsByteArrayAsync();
            return File(audioBytes, "audio/mpeg", enableRangeProcessing: true);
        }

        public async Task<IActionResult> SongDetail(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("SignIn", "Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var songResponse = await client.GetAsync($"https://localhost:7292/api/Song/{id}");
            if (!songResponse.IsSuccessStatusCode)
                return RedirectToAction("Charts");

            var songJson = await songResponse.Content.ReadAsStringAsync();
            var recommendationsJson = await client.GetStringAsync($"https://localhost:7292/api/Song/{id}/recommendations");

            var model = new SongDetailViewModel
            {
                Song = JsonConvert.DeserializeObject<ResultSongDto>(songJson),
                Recommendations = JsonConvert.DeserializeObject<List<ResultSongDto>>(recommendationsJson) ?? new(),
                UserPackageLevel = HttpContext.Session.GetInt32("PackageLevel") ?? 0,
                UserPackageName = HttpContext.Session.GetString("PackageName") ?? "Basic"
            };

            return View(model);
        }
    }
}

