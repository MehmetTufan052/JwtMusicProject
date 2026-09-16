namespace JwtMusicNight.WebUI.Dtos
{
    public class ResultArtistDto
    {
        public int ArtistId { get; set; }

        public string ArtistName { get; set; }

        public string ArtistImageUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public string Bio { get; set; }

        public string Country { get; set; }

        public bool IsVerified { get; set; }
    }
}
