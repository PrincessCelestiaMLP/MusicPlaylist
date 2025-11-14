using AutoMapper;
using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepo, IUserRepository userRepo, IMapper mapper)
    {
        _commentRepo = commentRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<CommentGetDto> CreateAsync(CommentCreateDto comment)
    {
        Comment commentCreate = _mapper.Map<Comment>(comment);
        await _commentRepo.CreateAsync(commentCreate);
        return _mapper.Map<CommentGetDto>(await GetAsync(commentCreate.Id));
    }

    public async Task<List<CommentGetDto>> GetAsync()
    {
        List<CommentGetDto> comments = _mapper.Map<List<CommentGetDto>>(await _commentRepo.GetAllAsync());

        //foreach (var comment in comments)
        //    comment.Author = 

        return comments;
    }

    public async Task<CommentGetDto> GetAsync(string id)
    {
        Comment? comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            throw new NullReferenceException($"Comment with id:{id} doesn't exist");

        CommentGetDto commentGet = _mapper.Map<CommentGetDto>(comment);
        //commentGet.Author = 

        return commentGet;
    }

    public async Task<List<CommentGetDto>> GetByPlaylistAsync(string id)
    {
        List<CommentGetDto> comments = _mapper.Map<List<CommentGetDto>>(await _commentRepo.GetByPlaylistId(id));

        //foreach (var comment in comments)
        //    comment.Author = 

        return comments;
    }

    public async Task DeleteAsync(string id)
    {
        if (await _commentRepo.GetByIdAsync(id) == null)
            throw new NullReferenceException($"Comment with id:{id} doesn't exist");

        await _commentRepo.DeleteAsync(id);
    }
}
