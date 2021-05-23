using FluentValidation;
using FriendOrganizer.Domain.Models;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Validator
{
    public class FriendPhoneNumberValidator : AbstractValidator<FriendPhoneNumber>
    {
        public FriendPhoneNumberValidator()
        {
            RuleFor(pn => pn.Number)
                .Must(BeAValidPhoneNumber)
                .WithMessage("Please enter a valid phone number");
        }

        private bool BeAValidPhoneNumber(string phoneNumber)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            if (phoneNumber.Length >= 7)
            {
                try
                {
                    var number = phoneNumberUtil.Parse(phoneNumber, null);
                    bool result = phoneNumberUtil.IsValidNumber(number);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
