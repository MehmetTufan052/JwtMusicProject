using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace JwtMusicNight.WebUI.Controllers
{
    public class ArtistController : Controller
    {
        IHttpClientFactory _httpClientFactory;

        public ArtistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> ArtistListV2()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("SignIn", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var responseMessage = await client.GetAsync("https://localhost:7292/api/Artist");

            if (!responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("SignIn", "Login");
            }

            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultArtistDto>>(jsonData);

            ViewBag.PackageName = HttpContext.Session.GetString("PackageName") ?? "Basic";

            return View(values);
        }

        public async Task<IActionResult> ArtistDetail(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("SignIn", "Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var artistResponse = await client.GetAsync("https://localhost:7292/api/Artist/" + id);

            if (!artistResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("ArtistListV2", "Artist");
            }

            var artistJson = await artistResponse.Content.ReadAsStringAsync();
            var songsJson = await client.GetStringAsync("https://localhost:7292/api/Song/byartist/" + id);

            var model = new ArtistDetailViewModel
            {
                Artist = JsonConvert.DeserializeObject<ResultArtistDto>(artistJson),
                Songs = JsonConvert.DeserializeObject<List<ResultSongDto>>(songsJson),
                UserPackageLevel = HttpContext.Session.GetInt32("PackageLevel") ?? 0,
                UserPackageName = HttpContext.Session.GetString("PackageName") ?? "Basic"
            };

            return View(model);
        }
    }
}
