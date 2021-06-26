using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Validator;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Model wrapper for the <see cref="Friend"/> class.
    /// </summary>
    public class FriendWrapper : ModelWrapper<Friend>
    {
        private readonly IValidator<FriendPhoneNumber> phoneValidator;

        /// <summary>
        /// Wraps the Model <see cref="Friend"/> and adds data validation.
        /// </summary>
        /// <param name="model">The Friend instance to be wrapped.</param>
        /// <param name="friendValidator">Optional FluentValidation <see cref="IValidator{T}"/> where T is <see cref="Friend"/>.</param>
        public FriendWrapper(Friend model, 
            IValidator<Friend> friendValidator = null,
            IValidator<FriendPhoneNumber> phoneValidator = null) : base(model, friendValidator)
        {
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
            this.phoneValidator = phoneValidator;
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
            Address = new AddressWrapper(model.Address);

            // Register all complex wrappers to enable property changes to bubble up.
            RegisterComplex<Address>(Address);
        }

        public int Id { get { return Model.Id; } }

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string FirstNameOriginalValue => GetOriginalValue<string>(nameof(FirstName));
        

        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));
        
        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LastNameOriginalValue => GetOriginalValue<string>(nameof(LastName));


        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));

        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string EmailOriginalValue => GetOriginalValue<string>(nameof(Email));

        public bool EmailIsChanged => GetIsChanged(nameof(Email));

        public AddressWrapper Address { get; private set; }

        public ChangeTrackingCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; private set; }

        public string FullName => GetValue<string>();

        // This is an optional field, so the int must be nullable.
        public int? FavoriteLanguageId
        {
            get { return GetValue<int?>(); }
            set { SetValue(value); }
        }

        // Override method to return true if any of the simple or complex properties have changes.
        public override bool IsChanged => base.IsChanged || Address.IsChanged;
    }
}