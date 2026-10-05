using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public AdminService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserSummaryDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Package)
                .ToListAsync();

            return users.Select(u => new UserSummaryDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                UserName = u.UserName,
                PackageId = u.PackageId,
                PackageName = u.Package.PackageName,
                PackageLevel = u.Package.Level
            }).ToList();
        }

        public async Task<bool> UpdateUserPackageAsync(string userId, int packageId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            var packageExists = await _context.Packages.AnyAsync(p => p.PackageId == packageId);
            if (!packageExists)
            {
                return false;
            }

            user.PackageId = packageId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ResultPackageDto>> GetAllPackagesAsync()
        {
            var packages = await _context.Packages.OrderBy(p => p.Level).ToListAsync();
            return _mapper.Map<List<ResultPackageDto>>(packages);
        }
    }
}
