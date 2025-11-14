using AutoMapper;
using MongoDB.Bson;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreateDto, User>()
            .AfterMap((src, dest) => {
                dest.Id = ObjectId.GenerateNewId().ToJson();
            });
        CreateMap<User, UserGetDto>()
            .ForMember(d => d.Playlists, opt => opt.Ignore());
    }
}
