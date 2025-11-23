using FluentValidation;
using MusicPlaylistAPI.Models.Dto.Create;

namespace MusicPlaylistAPI.Validators;

public class PlaylistValidator : AbstractValidator<PlaylistCreateDto>
{
    public PlaylistValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title is required");

        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .NotNull().WithMessage("UserId cannot be null");
    }
}
