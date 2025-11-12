using AutoMapper;
using MongoDB.Bson;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Mappers;

public class MusicProfile : Profile
{
    public MusicProfile()
    {
        CreateMap<MusicCreateDto, Music>()
            .AfterMap((src, dest) => {
                dest.Id = ObjectId.GenerateNewId().ToString();
            });
        CreateMap<Music, MusicGetDto>();
    }
}
