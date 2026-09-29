using JwtMusicNight.WebUI.Dtos;
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

            return new ContentResult
            {
                Content = body,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}

