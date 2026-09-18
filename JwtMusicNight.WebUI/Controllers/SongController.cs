using Microsoft.AspNetCore.Mvc;
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

