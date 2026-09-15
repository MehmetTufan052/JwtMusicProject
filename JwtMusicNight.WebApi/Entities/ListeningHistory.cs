namespace JwtMusicNight.WebApi.Entities
{
    public class ListeningHistory
    {
        public int ListeningHistoryId { get; set; }

        public DateTime ListenDate { get; set; }

        public int SongId { get; set; }

        public Song Song { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
    }
}
