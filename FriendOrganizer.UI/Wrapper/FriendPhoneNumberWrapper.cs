using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static PhoneNumbers.PhoneNumberUtil;
using PhoneValidationResult = PhoneNumbers.PhoneNumberUtil.ValidationResult;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Wraps the Model <see cref="FriendPhoneNumber"/> and adds data validation.
    /// </summary>
    /// <param name="model">The FriendPhoneNumber instance to be wrapped.</param>
    /// <param name="validator">Optional FluentValidation <see cref="IValidator{T}"/> where T is <see cref="FriendPhoneNumber"/>.</param>
    public class FriendPhoneNumberWrapper : ModelWrapper<FriendPhoneNumber>
    {
        public FriendPhoneNumberWrapper(FriendPhoneNumber model) : base(model)
        {
        }

        public string Number
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string NumberOriginalValue => GetOriginalValue<string>(nameof(Number));

        public bool NumberIsChanged => GetIsChanged(nameof(Number));

        public string Comment
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string CommentOriginalValue => GetOriginalValue<string>(nameof(Comment));

        public bool CommentIsChanged => GetIsChanged(nameof(Comment));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsAValidPhoneNumber(Number))
            {
                yield return new ValidationResult("Please enter a valid US phone number (with area code) or one in international format.",
                    new[] { nameof(Number) });
            };
        }

        private bool IsAValidPhoneNumber(string phoneNumber)
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
                    return result == PhoneValidationResult.IS_POSSIBLE;
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
