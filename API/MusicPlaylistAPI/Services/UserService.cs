using AutoMapper;
using MusicPlaylistAPI.Models.Dto;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IPlaylistService _playlistService;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepo, IPlaylistService playlistService ,IMapper mapper)
    {
        _userRepo = userRepo;
        _playlistService = playlistService;
        _mapper = mapper;
    }

    public async Task<UserGetDto> CreateAsync(UserCreateDto user)
    {
        User userCreate = _mapper.Map<User>(user);
        await _userRepo.CreateAsync(userCreate);
        return _mapper.Map<UserGetDto>(await GetAsync(userCreate.Id));
    }

    public async Task<List<UserGetDto>> GetAsync()
    {
        List<UserGetDto> users = _mapper.Map<List<UserGetDto>>(await _userRepo.GetAllAsync());
        for (int i = 0; i < users.Count; i++)
            users[i].Playlists = await _playlistService.GetByUserAsync(users[i].Id);

        return users;
    }

    public async Task<UserGetDto> GetAsync(string id)
    {
        User? user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            throw new NullReferenceException($"User with id:{id} doesn't exist");

        Task<List<PlaylistGetDto>> playlists = _playlistService.GetByUserAsync(user.Id);
        UserGetDto userGet = _mapper.Map<UserGetDto>(user);
        userGet.Playlists = await playlists;
        return userGet;
    }

    public async Task<UserGetDto> GetByEmailAsync(string email)
    {
        User? user = await _userRepo.GetByEmailAsync(email);
        if (user == null)
            throw new NullReferenceException($"User with email:{email} doesn't exist");

        Task<List<PlaylistGetDto>> playlists = _playlistService.GetByUserAsync(user.Id);
        UserGetDto userGet = _mapper.Map<UserGetDto>(user);
        userGet.Playlists = await playlists;
        return userGet;
    }

    public async Task<UserGetDto> UpdateAsync(string id, UserCreateDto user)
    {
        if (await _userRepo.GetByIdAsync(id) == null)
            throw new NullReferenceException($"User with id:{id} doesn't exist");

        User updateUser = _mapper.Map<User>(user);
        updateUser.Id = id;
        await _userRepo.UpdateAsync(id, updateUser);

        return _mapper.Map<UserGetDto>(await _userRepo.GetByIdAsync(id));
    }

    public async Task DeleteAsync(string id)
    {
        User? user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            throw new NullReferenceException($"User with id:{id} doesn't exist");

        Task del1 = _userRepo.DeleteAsync(id);

        List<PlaylistGetDto> playlists = (await _playlistService.GetByUserAsync(user.Id));
        foreach (var playlist in playlists)
            await _playlistService.DeleteAsync(playlist.Id);

        await del1;
    }
}
