namespace JwtMusicNight.WebApi.Entities
{
    public class Package
    {
        public int PackageId { get; set; }

        public string PackageName { get; set; }

        public int Level { get; set; }

        public List<Song> Songs { get; set; }

        public List<AppUser> Users { get; set; }
    }
}
