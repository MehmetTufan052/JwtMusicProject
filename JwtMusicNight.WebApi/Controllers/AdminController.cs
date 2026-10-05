using JwtMusicNight.WebApi.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var values = await _adminService.GetAllUsersAsync();
            return Ok(values);
        }

        [HttpPut("users/{id}/package")]
        public async Task<IActionResult> UpdateUserPackage(string id, int packageId)
        {
            var success = await _adminService.UpdateUserPackageAsync(id, packageId);
            if (!success)
            {
                return NotFound("Kullanıcı veya paket bulunamadı.");
            }
            return Ok();
        }

        [HttpGet("packages")]
        public async Task<IActionResult> GetPackages()
        {
            var values = await _adminService.GetAllPackagesAsync();
            return Ok(values);
        }
    }
}
