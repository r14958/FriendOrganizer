using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Validator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    [DebuggerDisplay("{Friend.Friend")]
    /// <summary>
    /// Model wrapper for the <see cref="Friend"/> class.
    /// </summary>
    public class FriendWrapper : ModelWrapper<Friend>
    {
        private readonly IValidator<FriendPhoneNumber> phoneValidator;
        private readonly IValidator<Address> addressValidator;

        /// <summary>
        /// Wraps the Model <see cref="Friend"/> and adds data validation.
        /// </summary>
        /// <param name="model">The Friend instance to be wrapped.</param>
        /// <param name="friendValidator">Optional FluentValidation <see cref="IValidator{T}"/> where T is <see cref="Friend"/>.</param>
        public FriendWrapper(Friend model, 
            IValidator<Friend> friendValidator = null,
            IValidator<FriendPhoneNumber> phoneValidator = null,
            IValidator<Address> addressValidator = null) : base(model, friendValidator)
        {
            this.phoneValidator = phoneValidator;
            this.addressValidator = addressValidator;
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
                model.PhoneNumbers.Select(pn => new FriendPhoneNumberWrapper(pn, phoneValidator)));

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
            Address = new AddressWrapper(model.Address, addressValidator);

            // Register all complex wrappers to enable property changes to bubble up.
            RegisterComplex<Address>(Address);
        }

        public int Id { get { return Model.Id; } }

        [Required(ErrorMessage = "Please specify a first name.")]
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
            if (IsDeveloper && (FavoriteLanguageId == null || FavoriteLanguageId <=0))
            {
                yield return new ValidationResult("Please select a favorite language for the developer.", 
                    new[] { nameof(IsDeveloper), nameof(FavoriteLanguageId) });
            }
        }
    }
}