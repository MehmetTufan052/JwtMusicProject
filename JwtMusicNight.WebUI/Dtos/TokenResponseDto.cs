namespace JwtMusicNight.WebUI.Dtos
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public string PackageName { get; set; }
        public int PackageLevel { get; set; }
        public bool IsAdmin { get; set; }
    }
}
