using JwtMusicNight.WebUI.Dtos;

namespace JwtMusicNight.WebUI.Models
{
    public class AdminUsersViewModel
    {
        public List<UserSummaryDto> Users { get; set; }
        public List<ResultPackageDto> Packages { get; set; }
    }
}
