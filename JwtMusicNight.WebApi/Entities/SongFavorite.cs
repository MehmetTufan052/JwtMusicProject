namespace JwtMusicNight.WebApi.Entities
{
    public class SongFavorite
    {
        public int SongFavoriteId { get; set; }

        public DateTime FavoriteDate { get; set; }

        public int SongId { get; set; }

        public Song Song { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
    }
}
