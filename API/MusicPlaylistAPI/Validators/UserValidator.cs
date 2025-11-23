using FluentValidation;
using MusicPlaylistAPI.Models.Dto.Create;

namespace MusicPlaylistAPI.Validators;

public class UserValidator : AbstractValidator<UserCreateDto>
{
    public UserValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(10).WithMessage("Username cannot be longer than 10 characters");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
