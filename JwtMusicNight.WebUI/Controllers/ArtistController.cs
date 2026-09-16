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
            var artist = JsonConvert.DeserializeObject<ResultArtistDto>(artistJson);

            var songResponse = await client.GetAsync("https://localhost:7292/api/Song/byartist/" + id);
            var songJson = await songResponse.Content.ReadAsStringAsync();
            var songs = JsonConvert.DeserializeObject<List<ResultSongDto>>(songJson);

            var model = new ArtistDetailViewModel
            {
                Artist = artist,
                Songs = songs ?? new List<ResultSongDto>(),
                UserPackageLevel = ReadClaimInt(token, "PackageLevel"),
                UserPackageName = ReadClaimString(token, "PackageName")
            };

            return View(model);
        }

        private static string ReadTokenPayload(string token)
        {
            var payload = token.Split('.')[1];
            payload = payload.Replace('-', '+').Replace('_', '/');

            if (payload.Length % 4 == 2) { payload += "=="; }
            else if (payload.Length % 4 == 3) { payload += "="; }

            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        }

        private static int ReadClaimInt(string token, string key)
        {
            try
            {
                var json = JObject.Parse(ReadTokenPayload(token));
                return int.Parse(json[key].ToString());
            }
            catch
            {
                return 0;
            }
        }

        private static string ReadClaimString(string token, string key)
        {
            try
            {
                var json = JObject.Parse(ReadTokenPayload(token));
                return json[key].ToString();
            }
            catch
            {
                return "Basic";
            }
        }
    }
}
