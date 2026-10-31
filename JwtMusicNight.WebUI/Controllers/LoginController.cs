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
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya e-posta ile şifreni girmelisin.";
                return View(loginDto);
            }

            loginDto.Username = loginDto.Username.Trim();

            try
            {
                var client = _httpClientFactory.CreateClient();
                var jsonData = JsonConvert.SerializeObject(loginDto);
                var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("http://localhost:5253/api/Login", stringContent);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                    var tokenModel = JsonConvert.DeserializeObject<TokenResponseDto>(jsonResponse);
                    if (tokenModel == null || string.IsNullOrWhiteSpace(tokenModel.Token))
                    {
                        ViewBag.ErrorMessage = "Giriş yanıtı alınamadı. Lütfen tekrar dene.";
                        return View(loginDto);
                    }

                    HttpContext.Session.SetString("JwtToken", tokenModel.Token);
                    HttpContext.Session.SetString("PackageName", tokenModel.PackageName);
                    HttpContext.Session.SetInt32("PackageLevel", tokenModel.PackageLevel);
                    HttpContext.Session.SetString("IsAdmin", tokenModel.IsAdmin.ToString());

                    return RedirectToAction("ArtistListV2", "Artist");
                }

                if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ViewBag.ErrorMessage = "Kullanıcı adı/e-posta veya şifre eşleşmiyor.";
                }
                else
                {
                    var errorBody = await responseMessage.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Giriş servisi hata verdi ({(int)responseMessage.StatusCode}). " +
                        (string.IsNullOrWhiteSpace(errorBody) ? "Lütfen tekrar dene." : errorBody);
                }
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Web API'ye bağlanılamadı. WebApi projesinin çalıştığından emin ol.";
            }

            return View(loginDto);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn", "Login");
        }
    }
}
