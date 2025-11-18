using AutoMapper;
using MongoDB.Bson;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Mappers;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CommentCreateDto, Comment>()
            .AfterMap((src, dest) => { 
                dest.Id = ObjectId.GenerateNewId().ToString();
                dest.CreatedAt = DateTime.Now;
            });
        CreateMap<Comment, FolllowGetDto>()
            .ForMember(d => d.Author, opt => opt.Ignore());
    }
}
