using FluentValidation;
using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Validator
{
    class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(a => a.City)
                .MaximumLength(50)
                .WithMessage("City name cannot exceed 50 characters.");

            RuleFor(a => a.Street)
                .MaximumLength(50)
                .WithMessage("Street name cannot exceed 50 characters.");

            RuleFor(a => a.StreetNumber)
                .MaximumLength(20)
                .WithMessage("Street number cannot exceed 20 characters.");
        }
    }
}
