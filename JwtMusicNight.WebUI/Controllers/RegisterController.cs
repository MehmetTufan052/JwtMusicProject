using JwtMusicNight.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace JwtMusicNight.WebUI.Controllers
{
  
    public class RegisterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterDto registerDto)
        {
            var client= _httpClientFactory.CreateClient();
            var jsonData=JsonConvert.SerializeObject(registerDto);
            StringContent stringContent=new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage=await client.PostAsync("https://localhost:7292/api/Register", stringContent);

            if(responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("SignIn", "Login");
            }
            return View();
        }
    }
}
