
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace JwtMusicNight.WebApi.Services.RegisterServices
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            AppUser appUser = new AppUser()
            {
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
                UserName = dto.Username,
                PackageId = 1
            };

            var result = await _userManager.CreateAsync(appUser, dto.Password);
            return result.Succeeded;
        }
    }
}
