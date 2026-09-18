namespace JwtMusicNight.WebApi.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string PackageName { get; set; }
        public int PackageLevel { get; set; }
    }
}
