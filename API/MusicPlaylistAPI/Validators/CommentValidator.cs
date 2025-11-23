using FluentValidation;
using MusicPlaylistAPI.Models.Dto.Create;

namespace MusicPlaylistAPI.Validators;

public class CommentValidator : AbstractValidator<CommentCreateDto>
{
    public CommentValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required");

        RuleFor(c => c.Text)
            .NotEmpty().WithMessage("Text is required");

        RuleFor(c => c.PlaylistId)
            .NotEmpty().WithMessage("PlaylistId is required")
            .NotNull().WithMessage("PlaylistId cannot be null");

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .NotNull().WithMessage("UserId cannot be null");
    }
}
