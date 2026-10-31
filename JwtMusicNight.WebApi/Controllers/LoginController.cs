using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.LoginServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginDto loginDto)
        {
            try
            {
                var result = await _loginService.LoginAsync(loginDto);

                if (result == null)
                {
                    return Unauthorized("Kullanıcı adı/e-posta veya şifre hatalı.");
                }

                return Ok(result);
            }
            catch (Exception exception)
            {
                return Problem(
                    title: "Giriş işlemi tamamlanamadı.",
                    detail: exception.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
