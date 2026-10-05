namespace JwtMusicNight.WebApi.Dtos
{
    public class ListeningHistoryDto
    {
        public int SongId { get; set; }
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string CoverImageUrl { get; set; }
        public string PackageName { get; set; }
        public int PackageLevel { get; set; }
        public DateTime ListenDate { get; set; }
    }
}
