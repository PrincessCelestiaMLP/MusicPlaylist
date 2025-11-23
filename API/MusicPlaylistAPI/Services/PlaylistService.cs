using AutoMapper;
using MusicPlaylistAPI.Models.Dto;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IMusicService _musicService;
    private readonly ICommentService _commentService;
    private readonly IFollowService _followService;
    private readonly IMapper _mapper;

    public PlaylistService(IPlaylistRepository playlistRepo, IMusicService musicService, ICommentService commentService,
        IFollowService followService, IMapper mapper)
    {
        _playlistRepo = playlistRepo;
        _musicService = musicService;
        _commentService = commentService;
        _followService = followService;
        _mapper = mapper;
    }

    public async Task<PlaylistGetDto> CreateAsync(PlaylistCreateDto playlist)
    {
        Playlist playlistCreate = _mapper.Map<Playlist>(playlist);
        await _playlistRepo.CreateAsync(playlistCreate);

        return _mapper.Map<PlaylistGetDto>(await GetAsync(playlistCreate.Id));
    }

    public async Task<List<PlaylistGetDto>> GetAsync()
    {
        List<PlaylistGetDto> playlists = _mapper.Map<List<PlaylistGetDto>>(await _playlistRepo.GetAllAsync());

        for (int i = 0; i < playlists.Count; i++)
        {
            Task<List<MusicGetDto>> musics = _musicService.GetByPlaylistAsync(playlists[i].Id);
            Task<List<CommentGetDto>> comments = _commentService.GetByPlaylistAsync(playlists[i].Id);
            Task<List<FollowGetDto>> follows = _followService.GetByPlaylistAsync(playlists[i].Id);

            playlists[i].Musics = await musics;
            playlists[i].Comments = await comments;
            playlists[i].Follows = await follows;
        }

        return playlists;
    }

    public async Task<PlaylistGetDto> GetAsync(string id)
    {
        Playlist? playlist = await _playlistRepo.GetByIdAsync(id);
        if (playlist == null)
            throw new NullReferenceException($"Playlist with id:{id} doesn't exist");

        PlaylistGetDto playlistGet = _mapper.Map<PlaylistGetDto>(playlist);

        Task<List<MusicGetDto>> musics = _musicService.GetByPlaylistAsync(playlistGet.Id);
        Task<List<CommentGetDto>> comments = _commentService.GetByPlaylistAsync(playlistGet.Id);
        Task<List<FollowGetDto>> follows = _followService.GetByPlaylistAsync(playlistGet.Id);

        playlistGet.Musics = await musics;
        playlistGet.Comments = await comments;
        playlistGet.Follows = await follows;

        return playlistGet;
    }

    public async Task<List<PlaylistGetDto>> GetByUserAsync(string id)
    {
        List<PlaylistGetDto> playlists = _mapper.Map<List<PlaylistGetDto>>(await _playlistRepo.GetByUserIdAsync(id));

        foreach (var playlist in playlists)
        {
            Task<List<MusicGetDto>> musics = _musicService.GetByPlaylistAsync(playlist.Id);
            Task<List<CommentGetDto>> comments = _commentService.GetByPlaylistAsync(playlist.Id);
            Task<List<FollowGetDto>> follows = _followService.GetByPlaylistAsync(playlist.Id);

            playlist.Musics = await musics;
            playlist.Comments = await comments;
            playlist.Follows = await follows;
        }

        return playlists;
    }

    public async Task<PlaylistGetDto> UpdateAsync(string id, PlaylistCreateDto playlist)
    {
        if (await _playlistRepo.GetByIdAsync(id) == null)
            throw new NullReferenceException($"Playlist with id:{id} doesn't exist");

        Playlist playlistCreate = _mapper.Map<Playlist>(playlist);
        playlistCreate.Id = id;
        await _playlistRepo.UpdateAsync(id, playlistCreate);

        return _mapper.Map<PlaylistGetDto>(playlist);
    }

    public async Task DeleteAsync(string id)
    {
        Playlist? playlist = await _playlistRepo.GetByIdAsync(id);
        if (playlist == null)
            throw new NullReferenceException($"Playlist with id:{id} doesn't exist");

        Task<List<MusicGetDto>> musics = _musicService.GetByPlaylistAsync(playlist.Id);
        Task<List<CommentGetDto>> comments = _commentService.GetByPlaylistAsync(playlist.Id);
        Task<List<FollowGetDto>> follows = _followService.GetByPlaylistAsync(playlist.Id);

        foreach (var music in await musics)
            await _musicService.DeleteAsync(music.Id);
        foreach (var comment in await comments)
            await _commentService.DeleteAsync(comment.Id);
        foreach (var follow in await follows)
            await _followService.DeleteAsync(follow.Id);
    }
}
