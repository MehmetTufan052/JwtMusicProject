using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.LoginServices
{
    public interface ILoginService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    }
}
