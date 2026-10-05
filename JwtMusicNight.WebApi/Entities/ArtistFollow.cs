namespace JwtMusicNight.WebApi.Entities
{
    public class ArtistFollow
    {
        public int ArtistFollowId { get; set; }

        public DateTime FollowDate { get; set; }

        public int ArtistId { get; set; }

        public Artist Artist { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
    }
}
