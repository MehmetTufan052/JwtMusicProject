namespace JwtMusicNight.WebApi.Entities
{
    public class Genre
    {
        public int GenreId { get; set; }

        public string GenreName { get; set; }

        public List<Song> Songs { get; set; }
    }
}
