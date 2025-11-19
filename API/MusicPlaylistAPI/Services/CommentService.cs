using AutoMapper;
using MusicPlaylistAPI.Models;
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
        List<Comment> comments = await _commentRepo.GetAllAsync();
        List<CommentGetDto> commentsGet = _mapper.Map<List<CommentGetDto>>(comments);

        for (int i = 0; i < comments.Count; i++)
        {
            User? user = await _userRepo.GetByIdAsync(comments[i].UserId);
            if (user == null)
                throw new ArgumentException($"User with id:{comments[i].UserId} doesn't exists");

            commentsGet[i].Author = _mapper.Map<UserView>(user);
        }

        return commentsGet;
    }

    public async Task<CommentGetDto> GetAsync(string id)
    {
        Comment? comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null)
            throw new NullReferenceException($"Comment with id:{id} doesn't exist");

        CommentGetDto commentGet = _mapper.Map<CommentGetDto>(comment);

        User? user = await _userRepo.GetByIdAsync(comment.UserId);
        if (user == null)
            throw new ArgumentException($"User with id:{comment.UserId} doesn't exists");
        commentGet.Author = _mapper.Map<UserView>(user);

        return commentGet;
    }

    public async Task<List<CommentGetDto>> GetByPlaylistAsync(string id)
    {
        List<Comment> comments = await _commentRepo.GetByPlaylistIdAsync(id);
        List<CommentGetDto> commentsGet = _mapper.Map<List<CommentGetDto>>(comments);

        for (int i = 0; i < comments.Count; i++)
        {
            User? user = await _userRepo.GetByIdAsync(comments[i].UserId);
            if (user == null)
                throw new ArgumentException($"User with id:{comments[i].UserId} doesn't exists");

            commentsGet[i].Author = _mapper.Map<UserView>(user);
        }

        return commentsGet;
    }

    public async Task DeleteAsync(string id)
    {
        if (await _commentRepo.GetByIdAsync(id) == null)
            throw new NullReferenceException($"Comment with id:{id} doesn't exist");

        await _commentRepo.DeleteAsync(id);
    }
}
