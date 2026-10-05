using JwtMusicNight.WebUI.Dtos;

namespace JwtMusicNight.WebUI.Models
{
    public class AdminSongsViewModel
    {
        public List<ResultSongDto> Songs { get; set; }
        public List<ResultArtistDto> Artists { get; set; }
        public List<ResultGenreDto> Genres { get; set; }
        public List<ResultPackageDto> Packages { get; set; }
        public ResultSongDto EditingSong { get; set; }
    }
}
