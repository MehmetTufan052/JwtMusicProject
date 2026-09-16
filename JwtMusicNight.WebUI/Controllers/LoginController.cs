using JwtMusicNight.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace JwtMusicNight.WebUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginDto loginDto)
        {
            var client= _httpClientFactory.CreateClient();
            var jsonData=JsonConvert.SerializeObject(loginDto);
            StringContent stringContent=new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage=await client.PostAsync("https://localhost:7292/api/Login", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonResponse=await responseMessage.Content.ReadAsStringAsync();
                var tokenModel=JsonConvert.DeserializeObject<TokenResponseDto>(jsonResponse);
                HttpContext.Session.SetString("JwtToken", tokenModel.Token);

                return RedirectToAction("ArtistListV2", "Artist");
            }
            ViewBag.ErrorMessage = "Yanlış kullanıcı adı veya şifre.";
            return View();
        }
    }
}
