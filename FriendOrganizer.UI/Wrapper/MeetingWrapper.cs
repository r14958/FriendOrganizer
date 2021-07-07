using FluentValidation;
using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    public class MeetingWrapper : ModelWrapper<Meeting>
    {
        private readonly IValidator<Friend> friendValidator;

        public MeetingWrapper(Meeting model, 
            IValidator<Meeting> meetingValidator = null,
            IValidator<Friend> friendValidator = null) : base(model, meetingValidator)
        {
            InitializeCollectionProperties(model);
            //InitializeComplexProperties(model);
            this.friendValidator = friendValidator;
        }

        //private void InitializeComplexProperties(Meeting model)
        //{
        //    DateFrom = new DateFromWrapper((DateTimeOffset)(model.DateFrom));
        //}

        public int Id { get { return Model.Id; } }

        public string Title
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string TitleOriginalValue => GetOriginalValue<string>(nameof(Title));


        public bool TitleIsChanged => GetIsChanged(nameof(Title));

        public DateTimeOffset DateFrom
        {
            get { return FixedDtoValue(GetValueOrDefault<DateTimeOffset>()); }
            set 
            { 
                SetValue(value);
                if(DateTo < DateFrom)
                {
                    DateTo = DateFrom;
                }
            }
        }

        public DateTimeOffset DateFromOriginalValue => GetOriginalValue<DateTimeOffset>(nameof(DateFrom));

        public bool DateFromIsChanged => GetIsChanged(nameof(DateFrom));

        public DateTimeOffset DateTo
        {
            get { return FixedDtoValue(GetValueOrDefault<DateTimeOffset>()); }
            set
            {
                SetValue(value);
                if (DateFrom > DateTo)
                {
                    DateFrom = DateTo;
                }
            }
        }

        public DateTimeOffset DateToOriginalValue => GetOriginalValue<DateTimeOffset>(nameof(DateTo));

        public bool DateToIsChanged => GetIsChanged(nameof(DateTo));

        private static DateTimeOffset FixedDtoValue(DateTimeOffset input)
        {

            // If a non-default value is inputted, return the same value.
            if (input != DateTimeOffset.MinValue) return input;
            
            // Otherwise, return Now, rounded up the next half hour.
            var now = DateTimeOffset.Now;

            if (now.Minute <= 30)
            {
                return new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 30, 0, now.Offset); 
            }

            return new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Offset).AddHours(1);

        }

        public ChangeTrackingCollection<FriendWrapper> AddedFriends { get; private set; }

        private void InitializeCollectionProperties(Meeting model)
        {
            if (model.Friends == null)
            {
                throw new ArgumentException("Added Friends cannot be null.");
            }
            AddedFriends = new ChangeTrackingCollection<FriendWrapper>(
               model.Friends.Select(f => new FriendWrapper(f, friendValidator)));
            

            // Using a method in the base class, register the two model and wrapper collections to keep them in sync.
            RegisterCollection(AddedFriends, model.Friends.ToList());
        }
    }
}
