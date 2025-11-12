using AutoMapper;
using MongoDB.Bson;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Mappers;

public class FollowProfile : Profile
{
    public FollowProfile()
    {
        CreateMap<FollowCreteDto, Follow>()
            .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.FollowerId))
            .AfterMap((src, dest) => {
                dest.Id = ObjectId.GenerateNewId().ToString();
                dest.FollowedAt = DateTime.Now;
            });
        CreateMap<Follow, FollowGetDto>()
            .ForMember(d => d.Follower, opt => opt.Ignore());
    }
}
