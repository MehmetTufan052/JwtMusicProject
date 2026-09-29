using JwtMusicNight.WebUI.Dtos;

namespace JwtMusicNight.WebUI.Models
{
    public class GenrePageViewModel
    {
        public List<ResultGenreDto> Genres { get; set; } = new();
        public List<ResultSongDto> Songs { get; set; } = new();
        public string UserPackageName { get; set; } = "Basic";
        public int UserPackageLevel { get; set; }
    }
}
