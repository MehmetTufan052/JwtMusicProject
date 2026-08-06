using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.RegisterServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterDto registerDto)
        {
            await _registerService.RegisterAsync(registerDto);
            return Ok("Başarılı!");
        }
    }
}
