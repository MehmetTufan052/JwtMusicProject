using AutoMapper;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;

namespace JwtMusicNight.WebApi.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Song, ResultSongDto>()
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist.ArtistName))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.GenreName))
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.PackageName))
                .ForMember(dest => dest.PackageLevel, opt => opt.MapFrom(src => src.Package.Level));

            CreateMap<Song, CreateSongDto>().ReverseMap();

            CreateMap<Artist, ResultArtistDto>().ReverseMap();
            CreateMap<Artist, CreateArtistDto>().ReverseMap();

            CreateMap<Genre, ResultGenreDto>().ReverseMap();
            CreateMap<Genre, CreateGenreDto>().ReverseMap();

            CreateMap<Package, ResultPackageDto>();

            CreateMap<Playlist, ResultPlaylistDto>()
                .ForMember(dest => dest.SongCount, opt => opt.MapFrom(src => src.PlaylistSongs.Count));

            CreateMap<Playlist, PlaylistDetailDto>()
                .ForMember(dest => dest.Songs, opt => opt.MapFrom(src => src.PlaylistSongs.Select(ps => ps.Song)));

            CreateMap<ListeningHistory, ListeningHistoryDto>()
                .ForMember(dest => dest.SongName, opt => opt.MapFrom(src => src.Song.SongName))
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Song.Artist.ArtistName))
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.Song.CoverImageUrl))
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Song.Package.PackageName))
                .ForMember(dest => dest.PackageLevel, opt => opt.MapFrom(src => src.Song.Package.Level));
        }
    }
}
