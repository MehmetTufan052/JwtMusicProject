using JwtMusicNight.WebUI.Dtos;

namespace JwtMusicNight.WebUI.Models
{
    public class ArtistDetailViewModel
    {
        public ResultArtistDto Artist { get; set; }
        public List<ResultSongDto> Songs { get; set; }
        public int UserPackageLevel { get; set; }
        public string UserPackageName { get; set; }
    }
}
