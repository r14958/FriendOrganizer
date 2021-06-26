using FluentValidation;
using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Validator
{
    public class MeetingValidator : AbstractValidator<Meeting>
    {
        public MeetingValidator()
        {
            RuleFor(m => m.Title).NotEmpty().WithMessage("A title is required.")
                .MaximumLength(50).WithMessage("The meeting title cannot exceed 50 characters.");
        }    
    }
}
