namespace JwtMusicNight.WebApi.Entities
{
    public class Song
    {
        public int SongId { get; set; }

        public string SongName { get; set; }

        public string CoverImageUrl { get; set; }

        public string AudioUrl { get; set; }

        public TimeSpan Duration { get; set; }

        public int ListenCount { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Lyrics { get; set; }


        public int ArtistId { get; set; }

        public Artist Artist { get; set; }
        

        public int GenreId { get; set; }

        public Genre Genre { get; set; }


        public int PackageId { get; set; }

        public Package Package { get; set; }

    }
}
