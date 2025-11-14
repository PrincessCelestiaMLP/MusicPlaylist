using AutoMapper;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Services;

public class MusicService : IMusicService
{
    private readonly IMusicRepository _musicRepo;
    private readonly IMapper _mapper;

    public MusicService(IMusicRepository musicRepo, IMapper mapper)
    {
        _musicRepo = musicRepo;
        _mapper = mapper;
    }

    public async Task<MusicGetDto> CreateAsync(MusicCreateDto music)
    {
        Music musicCreate = _mapper.Map<Music>(music);
        await _musicRepo.CreateAsync(musicCreate);
        return _mapper.Map<MusicGetDto>(await GetAsync(musicCreate.Id));
    }

    public async Task<List<MusicGetDto>> GetAsync() =>
        _mapper.Map<List<MusicGetDto>>(await _musicRepo.GetAllAsync());

    public async Task<MusicGetDto> GetAsync(string id)
    {
        Music? music = await _musicRepo.GetByIdAsync(id);
        if (music == null)
            throw new NullReferenceException($"Music with id:{id} doesn't exist");

        return _mapper.Map<MusicGetDto>(music);
    }

    public async Task<List<MusicGetDto>> GetByPlaylistAsync(string id) =>
        _mapper.Map<List<MusicGetDto>>(await _musicRepo.GetByPlaylistIdAsync(id));

    public async Task<MusicGetDto> UpdateAsync(string id, MusicCreateDto music)
    {
        if (await _musicRepo.GetByIdAsync(id) == null)
            throw new NullReferenceException($"Music with id:{id} doesn't exist");

        Music musicUpdate = _mapper.Map<Music>(music);
        musicUpdate.Id = id;
        await _musicRepo.UpdateAsync(id, musicUpdate);

        return _mapper.Map<MusicGetDto>(musicUpdate);
    }

    public async Task DeleteAsync(string id)
    {
        if (await _musicRepo.GetByIdAsync(id) == null)
            throw new NullReferenceException($"Music with id:{id} doesn't exist");

        await _musicRepo.DeleteAsync(id);
    }
}
