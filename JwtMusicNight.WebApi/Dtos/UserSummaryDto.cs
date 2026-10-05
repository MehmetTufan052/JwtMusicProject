namespace JwtMusicNight.WebApi.Dtos
{
    public class UserSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int PackageLevel { get; set; }
    }
}
