using AutoMapper;
using MusicPlaylistAPI.Models;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepo;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public FollowService(IFollowRepository followRepo, IUserRepository userRepo, IMapper mapper)
    {
        _followRepo = followRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<FollowGetDto> CreateAsync(FollowCreteDto follow)
    {
        Follow followCreate = _mapper.Map<Follow>(follow);
        await _followRepo.CreateAsync(followCreate);
        return _mapper.Map<FollowGetDto>(await GetAsync(followCreate.Id));
    }

    public async Task<List<FollowGetDto>> GetAsync()
    {
        List<Follow> follows = await _followRepo.GetAllAsync();
        List<FollowGetDto> followsGet = _mapper.Map<List<FollowGetDto>>(follows);

        for (int i = 0; i < follows.Count; i++)
        {
            User? user = await _userRepo.GetByIdAsync(follows[i].UserId);
            if (user == null)
                throw new ArgumentException($"User with id:{follows[i].UserId} doesn't exists");

            followsGet[i].Follower = _mapper.Map<UserView>(user);
        }

        return followsGet;
    }

    public async Task<FollowGetDto> GetAsync(string id)
    {
        Follow? follow = await _followRepo.GetByIdAsync(id);
        if (follow == null)
            throw new NullReferenceException($"Follow with id:{id} doesn't exist");

        FollowGetDto followGet = _mapper.Map<FollowGetDto>(follow);

        User? user = await _userRepo.GetByIdAsync(follow.UserId);
        if (user == null)
            throw new ArgumentException($"User with id:{follow.UserId} doesn't exists");
        followGet.Follower = _mapper.Map<UserView>(user);

        return followGet;
    }

    public async Task<List<FollowGetDto>> GetByPlaylistAsync(string id)
    {
        List<Follow> follows = await _followRepo.GetByPlaylistIdAsync(id);
        List<FollowGetDto> followsGet = _mapper.Map<List<FollowGetDto>>(follows);

        for (int i = 0; i < follows.Count; i++)
        {
            User? user = await _userRepo.GetByIdAsync(follows[i].UserId);
            if (user == null)
                throw new ArgumentException($"User with id:{follows[i].UserId} doesn't exists");

            followsGet[i].Follower = _mapper.Map<UserView>(user);
        }

        return followsGet;
    }

    public async Task DeleteAsync(string id)
    {
        if (await _followRepo.GetByIdAsync(id) == null)
            throw new NullReferenceException($"Follow with id:{id} doesn't exist");

        await _followRepo.DeleteAsync(id);
    }
}
