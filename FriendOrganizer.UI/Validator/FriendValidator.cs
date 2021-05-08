using FluentValidation;
using FriendOrganizer.Domain.Models;
using System;

namespace FriendOrganizer.UI.Validator
{
    public class FriendValidator : AbstractValidator<Friend>
    {
        public FriendValidator()
        {
            RuleFor(f => f.FirstName)
                .NotEmpty()
                .WithMessage("Please specify a first name.")
                .MinimumLength(2)
                .WithMessage("First name must be at least two characters.")
                .MaximumLength(50)
                .WithMessage("First name cannot exceed 50 characters.")
                .NotEqual("Robot", StringComparer.OrdinalIgnoreCase)
                .WithMessage("Robots are not valid Friends.");

            RuleFor(f => f.LastName)
                .NotEmpty()
               .WithMessage("Please specify a last name.")
               .MinimumLength(2)
               .WithMessage("Last name must be at least two characters.")
               .MaximumLength(50)
               .WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(f => f.Email)
                .EmailAddress()
                .WithMessage("Please enter a valid email address.")
                .When(f => f.Email != null);
        }
    }
}