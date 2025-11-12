using AutoMapper;
using MongoDB.Bson;
using MusicPlaylistAPI.Models.Dto;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Mappers;

public class PlaylistProfile : Profile
{
    public PlaylistProfile()
    {
        CreateMap<PlaylistCreateDto, Playlist>()
            .AfterMap((src, dest) => {
                dest.Id = ObjectId.GenerateNewId().ToString();
                dest.CreatedAt = DateTime.Now;
            });
        CreateMap<Playlist, PlaylistGetDto>()
            .ForMember(d => d.Musics, opt => opt.Ignore())
            .ForMember(d => d.Comments, opt => opt.Ignore())
            .ForMember(d => d.Follows, opt => opt.Ignore());
    }
}
