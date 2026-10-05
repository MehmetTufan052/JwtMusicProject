using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtMusicNight.WebApi.Services.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtContext _context;

        public LoginService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, JwtContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return null;
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(p => p.PackageId == user.PackageId);

            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponseDto
            {
                Token = await GenerateToken(user, package),
                PackageName = package.PackageName,
                PackageLevel = package.Level,
                IsAdmin = roles.Contains("Admin")
            };
        }

        private async Task<string> GenerateToken(AppUser appUser, Package package)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var roles = await _userManager.GetRolesAsync(appUser);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, appUser.Id),
        new Claim(ClaimTypes.Email, appUser.Email),
        new Claim(ClaimTypes.Name, appUser.Name),
        new Claim(ClaimTypes.Surname, appUser.Surname),
        new Claim("PackageLevel", package.Level.ToString()),
        new Claim("PackageName", package.PackageName),
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
