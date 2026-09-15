using AutoMapper;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;

namespace JwtMusicNight.WebApi.Mapping
{
    public class GeneralMapping: Profile
    {
        public GeneralMapping()
        {
            CreateMap<Song, ResultSongDto>().ReverseMap();
            CreateMap<Song, CreateSongDto>().ReverseMap();

            CreateMap<Artist, ResultArtistDto>().ReverseMap();
            CreateMap<Artist, CreateArtistDto>().ReverseMap();

            CreateMap<Genre, ResultGenreDto>().ReverseMap();
        }
    }
}
