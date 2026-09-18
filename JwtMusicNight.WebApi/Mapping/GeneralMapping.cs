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
        }
    }
}
