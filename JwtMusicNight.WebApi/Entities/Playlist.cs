namespace JwtMusicNight.WebApi.Entities
{
    public class Playlist
    {
        public int PlaylistId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }

        public List<PlaylistSong> PlaylistSongs { get; set; }
    }
}
