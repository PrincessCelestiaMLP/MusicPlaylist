using FluentValidation;
using MusicPlaylistAPI.Models.Dto.Create;

namespace MusicPlaylistAPI.Validators;

public class FollowValidator : AbstractValidator<FollowCreteDto>
{
    public FollowValidator()
    {
        RuleFor(f => f.PlaylistId)
            .NotEmpty().WithMessage("PlaylistId is required")
            .NotNull().WithMessage("PlaylistId cannot be null");

        RuleFor(f => f.FollowerId)
            .NotEmpty().WithMessage("FollowerId is required")
            .NotNull().WithMessage("FollowerId cannot be null");
    }
}
