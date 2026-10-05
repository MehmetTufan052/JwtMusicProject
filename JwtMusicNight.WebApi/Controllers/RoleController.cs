using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            IdentityRole identityRole = new IdentityRole(roleName);
            await _roleManager.CreateAsync(identityRole);
            return Ok("Rol oluşturuldu!");
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(string username, string roleName)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return NotFound("Rol bulunamadı.");
            }

            await _userManager.AddToRoleAsync(user, roleName);
            return Ok("Rol atandı!");
        }
    }
}
