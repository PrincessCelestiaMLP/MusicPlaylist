using FluentValidation;
using MusicPlaylistAPI.Models.Dto.Create;

namespace MusicPlaylistAPI.Validators;

public class MusicValidator : AbstractValidator<MusicCreateDto>
{
    public MusicValidator()
    {
        RuleFor(m => m.Title)
            .NotEmpty().WithMessage("Title is required");

        RuleFor(m => m.Artist)
            .NotEmpty().WithMessage("Artist is required");

        RuleFor(m => m.PlaylistId)
            .NotEmpty().WithMessage("PlaylistId is required")
            .NotNull().WithMessage("PlaylistId cannot be null");

        RuleFor(m => m.Link)
            .NotEmpty().WithMessage("Link is required");
    }
}
