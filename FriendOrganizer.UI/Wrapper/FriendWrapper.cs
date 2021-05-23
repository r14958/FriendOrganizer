using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Validator;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Model wrapper for the <see cref="Friend"/> class.
    /// </summary>
    public class FriendWrapper : ModelWrapper<Friend>
    {
        /// <summary>
        /// Wraps the Model <see cref="Friend"/> and adds data validation.
        /// </summary>
        /// <param name="model">The Friend instance to be wrapped.</param>
        /// <param name="validator">Optional FluentValidation <see cref="IValidator{T}"/> where T is <see cref="Friend"/>.</param>
        public FriendWrapper(Friend model, IValidator<Friend> validator = null) : base(model, validator)
        {
        }

        public int Id { get { return Model.Id; } }

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        
        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string FullName => GetValue<string>();

        // This is an optional field, so the int must be nullable.
        public int? FavoriteLanguageId
        {
            get { return GetValue<int?>(); }
            set { SetValue(value); }
        }
    }
}