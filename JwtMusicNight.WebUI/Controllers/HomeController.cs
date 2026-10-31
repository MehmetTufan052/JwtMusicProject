using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JwtMusicNight.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var artists = new List<JwtMusicNight.WebUI.Dtos.ResultArtistDto>();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://localhost:5253/api/Artist");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    artists = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JwtMusicNight.WebUI.Dtos.ResultArtistDto>>(json)
                              ?? new List<JwtMusicNight.WebUI.Dtos.ResultArtistDto>();
                }
            }
            catch (HttpRequestException)
            {
                // API erişilemiyorsa ana sayfanın geri kalanı açılmaya devam eder.
            }

            return View(artists);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
