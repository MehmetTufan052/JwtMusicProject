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
            var loginValue = loginDto.Username?.Trim();
            if (string.IsNullOrWhiteSpace(loginValue) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return null;
            }

            var user = await _userManager.FindByNameAsync(loginValue);

            if (user == null && loginValue.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(loginValue);
            }

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

            // Eski veya eksik paket ilişkisi olan hesapların girişte 500 hatası vermesini engelle.
            if (package == null)
            {
                package = await _context.Packages.OrderBy(p => p.Level).FirstOrDefaultAsync();
                if (package == null)
                {
                    return null;
                }

                user.PackageId = package.PackageId;
                await _userManager.UpdateAsync(user);
            }

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

            var jwtKey = jwtSettings["Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT imzalama anahtarı bulunamadı.");
            }

            var issuer = jwtSettings["Issuer"] ?? "MusicAppApi";
            var audience = jwtSettings["Audience"] ?? "MusicAppUsers";
            var expireMinutes = jwtSettings.GetValue<int?>("ExpireMinutes") ?? 20;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(ClaimTypes.Name, appUser.Name ?? appUser.UserName ?? "Kullanıcı"),
                new Claim(ClaimTypes.Surname, appUser.Surname ?? string.Empty),
                new Claim("PackageLevel", package.Level.ToString()),
                new Claim("PackageName", package.PackageName ?? "Free"),
            };

            if (!string.IsNullOrWhiteSpace(appUser.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, appUser.Email));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
