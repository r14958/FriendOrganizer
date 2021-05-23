using FluentValidation;
using FriendOrganizer.Domain.Models;

namespace FriendOrganizer.UI.Validator
{
    public class ProgrammingLanguageValidator : AbstractValidator<ProgrammingLanguage>
    {
        public ProgrammingLanguageValidator()
        {
            RuleFor(pl => pl.Name)
                .MaximumLength(50)
                .WithMessage("First name cannot exceed 50 characters.");
        }
    }
}
