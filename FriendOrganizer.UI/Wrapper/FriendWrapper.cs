using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace FriendOrganizer.UI.Wrapper
{
    [DebuggerDisplay("{Friend.Friend")]
    /// <summary>
    /// Model wrapper for the <see cref="Friend"/> class.
    /// </summary>
    public class FriendWrapper : ModelWrapper<Friend>
    {
        private const int MinimumFirstNameLength = 2;
        private const int MaximumFirstNameLength = 50;
        private const int MinimumLastNameLength = 2;
        private const int MaximumLastNameLength = 50;


        /// <summary>
        /// Wraps the Model <see cref="Friend"/> and adds data validation.
        /// </summary>
        /// <param name="model">The Friend instance to be wrapped.</param>
        /// <param name="friendValidator">Optional FluentValidation <see cref="IValidator{T}"/> where T is <see cref="Friend"/>.</param>
        public FriendWrapper(Friend model) : base(model)
        {
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
        }

        /// <summary>
        /// Wraps every member of a collection property of a model and then "registers" both the
        /// wrapper and model collections to keep them sync and bubble up change notifications.
        /// </summary>
        /// <param name="model">The model that contains the collection properties.</param>
        private void InitializeCollectionProperties(Friend model)
        {
            if (model.PhoneNumbers == null )
            {
                throw new ArgumentException("PhoneNumbers cannot be null.");
            }
            PhoneNumbers = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(
                model.PhoneNumbers.Select(pn => new FriendPhoneNumberWrapper(pn)));

            // Using a method in the base class, register the two model and wrapper collections to keep them in sync.
            RegisterCollection(PhoneNumbers, model.PhoneNumbers);
        }

        /// <summary>
        /// Wraps every member of a complex (e.g., object) property of a model and then "registers" 
        /// the "complex" wrapper to bubble up change notifications.
        /// </summary>
        /// <param name="model">The model that contains the collection properties.</param>
        private void InitializeComplexProperties(Friend model)
        {
            if (model.Address == null)
            {
                throw new ArgumentException("Address cannot be null.");
            }
            
            // Wrap all complex model properties in their own model wrappers.
            Address = new AddressWrapper(model.Address);

            // Register all complex wrappers to enable property changes to bubble up.
            RegisterComplex<Address>(Address);
        }

        public int Id { get { return Model.Id; } }

        public string FirstName
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string FirstNameOriginalValue => GetOriginalValue<string>(nameof(FirstName));
        

        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));
        
        public string LastName
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string LastNameOriginalValue => GetOriginalValue<string>(nameof(LastName));


        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));

        public string Email
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string EmailOriginalValue => GetOriginalValue<string>(nameof(Email));

        public bool EmailIsChanged => GetIsChanged(nameof(Email));

        public AddressWrapper Address { get; private set; }

        public bool IsDeveloper
        {
            get { return GetValueOrDefault<bool>(); }
            set { SetValue(value); }
        }

        public bool IsDeveloperOriginalValue => GetOriginalValue<bool>(nameof(IsDeveloper));

        public bool IsDeveloperIsChanged => GetIsChanged(nameof(IsDeveloper));

        public ChangeTrackingCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; private set; }

        public string FullName => GetValueOrDefault<string>();

        // This is an optional field, so the int must be nullable.
        public int? FavoriteLanguageId
        {
            get { return GetValueOrDefault<int?>(); }
            set { SetValue(value); }
        }

        public int FavoriteLanguageIdOriginalValue => GetOriginalValue<int>(nameof(FavoriteLanguageId));

        public bool FavoriteLanguageIdIsChanged => GetIsChanged(nameof(FavoriteLanguageId));

        // Override method to return true if any of the simple or complex properties have changes.
        public override bool IsChanged => base.IsChanged || Address.IsChanged;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("Please specify a first name.",
                    new[] { nameof(FirstName) });
            }
            if (FirstName.Length < MinimumFirstNameLength)
            {
                yield return new ValidationResult($"First name must be at least {MinimumFirstNameLength} characters.",
                    new[] { nameof(FirstName) });
            }
            if (FirstName.Length > MaximumFirstNameLength)
            {
                yield return new ValidationResult($"First name cannot exceed {MaximumFirstNameLength} characters.",
                                   new[] { nameof(FirstName) });
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("Please specify a last name.",
                    new[] { nameof(LastName) });
            }
            if (LastName.Length < MinimumLastNameLength)
            {
                yield return new ValidationResult($"Last name must be at least {MinimumLastNameLength} characters.",
                                   new[] { nameof(LastName) });
            }
            if (LastName.Length > MaximumLastNameLength)
            {
                yield return new ValidationResult($"Last name cannot exceed {MaximumLastNameLength} characters.",
                                   new[] { nameof(LastName) });
            }
            if (Email != null && !new EmailAddressAttribute().IsValid(Email))
            {
                yield return new ValidationResult($"Please enter a valid email address.",
                                   new[] { nameof(Email) });
            }
            if (IsDeveloper && (FavoriteLanguageId == null || FavoriteLanguageId <=0))
            {
                yield return new ValidationResult("Please select a favorite language for the developer.", 
                    new[] { nameof(IsDeveloper), nameof(FavoriteLanguageId) });
            }
        }
    }
}