using FluentValidation;
using FriendOrganizer.Domain.Models;

namespace FriendOrganizer.UI.Validator
{
    public class ProgrammingLanguageValidator : AbstractValidator<ProgrammingLanguage>
    {
        public ProgrammingLanguageValidator()
        {
            RuleFor(pl => pl.Name)
                .NotEmpty()
                .WithMessage("A name for the programming language is required.")
                .MaximumLength(50)
                .WithMessage("Name cannot exceed 50 characters.");
        }
    }
}
