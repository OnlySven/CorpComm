using FluentValidation;

namespace CorpComm.Application.Features.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email є обов'язковим.")
            .EmailAddress().WithMessage("Невірний формат Email.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ім'я є обов'язковим.")
            .MinimumLength(2).WithMessage("Ім'я має містити мінімум 2 символи.")
            .MaximumLength(100).WithMessage("Ім'я не може перевищувати 100 символів.");
    }
}