using FluentValidation;
using FriendOrganizer.Domain.Models;
using PhoneNumbers;
using System;
using static PhoneNumbers.PhoneNumberUtil;

namespace FriendOrganizer.UI.Validator
{
    public class PhoneValidator : AbstractValidator<FriendPhoneNumber>
    {
        public PhoneValidator()
        {
            RuleFor(pn => pn.Number)
                .Must(BeAValidPhoneNumber)
                .WithMessage("Please enter a valid US phone number (with area code) or one in international format.");
        }

        private bool BeAValidPhoneNumber(string phoneNumber)
        {
            var phoneNumberUtil = GetInstance();

            // Only try parsing if at least six characters have been entered.
            if (phoneNumber != null && phoneNumber.Length >= 6)
            {
                try
                {
                    // Users will have to input phone numbers is a variety of acceptable US formats
                    // (with area code) or in proper International format (starting with +CountryCode).
                    var number = phoneNumberUtil.Parse(phoneNumber, "US");

                    // Using the more forgiving "IsPossibleNumber" option.
                    var result = phoneNumberUtil.IsPossibleNumberWithReason(number);

                    // However, will return true for only one particular response.
                    return result == ValidationResult.IS_POSSIBLE;
                }
                catch (Exception)
                {
                    // If the parsing fails, not a valid phone number.
                    return false;
                }
            }
            return false;
        }
    }
}
