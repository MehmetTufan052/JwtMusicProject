namespace JwtMusicNight.WebApi.Entities
{
    public class Artist
    {
        public int ArtistId { get; set; }

        public string ArtistName { get; set; }

        public string ArtistImageUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public string Bio { get; set; }

        public string Country { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsVerified { get; set; }

        // Navigation Property
        public List<Song> Songs { get; set; }
    }
}
