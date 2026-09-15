using Microsoft.AspNetCore.Identity;

namespace JwtMusicNight.WebApi.Entities
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; }

        public List<ListeningHistory> ListeningHistories { get; set; }
    }
}
