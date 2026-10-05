using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.AdminServices
{
    public interface IAdminService
    {
        Task<List<UserSummaryDto>> GetAllUsersAsync();
        Task<bool> UpdateUserPackageAsync(string userId, int packageId);
        Task<List<ResultPackageDto>> GetAllPackagesAsync();
    }
}
