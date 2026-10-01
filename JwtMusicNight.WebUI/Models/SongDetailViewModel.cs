using JwtMusicNight.WebUI.Dtos;

namespace JwtMusicNight.WebUI.Models
{
    public class SongDetailViewModel
    {
        public ResultSongDto Song { get; set; }
        public List<ResultSongDto> Recommendations { get; set; }
        public int UserPackageLevel { get; set; }
        public string UserPackageName { get; set; }
    }
}
