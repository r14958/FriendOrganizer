using FluentValidation;
using FriendOrganizer.Domain.Models;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Wraps the Model <see cref="FriendPhoneNumber"/> and adds data validation.
    /// </summary>
    /// <param name="model">The FriendPhoneNumber instance to be wrapped.</param>
    /// <param name="validator">Optional FluentValidation <see cref="IValidator{T}"/> where T is <see cref="FriendPhoneNumber"/>.</param>
    public class FriendPhoneNumberWrapper : ModelWrapper<FriendPhoneNumber>
    {
        public FriendPhoneNumberWrapper(FriendPhoneNumber model, IValidator<FriendPhoneNumber> validator = null) : base(model, validator)
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
    }
}
