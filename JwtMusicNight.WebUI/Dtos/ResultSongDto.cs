namespace JwtMusicNight.WebUI.Dtos
{
    public class ResultSongDto
    {
        public int SongId { get; set; }
        public string SongName { get; set; }
        public string CoverImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public int ListenCount { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string GenreName { get; set; }
        public string PackageName { get; set; }
        public int PackageLevel { get; set; }
    }
}
