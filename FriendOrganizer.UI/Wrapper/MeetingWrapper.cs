﻿using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    public class MeetingWrapper : ModelWrapper<Meeting>
    {
        private const int MinimumTitleLength = 2;
        private const int MaximumTitleLength = 50;

        public MeetingWrapper(Meeting model) : base(model)
        {
            InitializeCollectionProperties(model);
        }

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
               model.Friends.Select(f => new FriendWrapper(f)));
            

            // Using a method in the base class, register the two model and wrapper collections to keep them in sync.
            RegisterCollection(AddedFriends, model.Friends.ToList());
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                yield return new ValidationResult("Please specify a meeting title.",
                    new[] { nameof(Title) });
            }
            if (Title.Length < MinimumTitleLength)
            {
                yield return new ValidationResult($"City name must be at least {MinimumTitleLength} characters.",
                    new[] { nameof(Title) });
            }
            if (Title.Length > MaximumTitleLength)
            {
                yield return new ValidationResult($"City name cannot exceed {MaximumTitleLength} characters.",
                                   new[] { nameof(Title) });
            }
        }
    }
}
