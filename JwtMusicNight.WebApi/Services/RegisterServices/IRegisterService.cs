using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.RegisterServices
{
    public interface IRegisterService
    {
        public Task<bool> RegisterAsync(RegisterDto registerDto);
    }
}
