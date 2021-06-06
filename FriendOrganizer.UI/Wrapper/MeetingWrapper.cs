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
        public MeetingWrapper(Meeting model, IValidator<Meeting> validator) : base(model, validator)
        {
        }

        public int Id { get { return Model.Id; } }

        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


        public DateTimeOffset DateFrom
        {
            get { return FixedDtoValue(GetValue<DateTimeOffset>()); }
            set 
            { 
                SetValue(value);
                if(DateTo < DateFrom)
                {
                    DateTo = DateFrom;
                }
            }
        }

        public DateTimeOffset DateTo
        {
            get { return FixedDtoValue(GetValue<DateTimeOffset>()); }
            set
            {
                SetValue(value);
                if (DateFrom > DateTo)
                {
                    DateFrom = DateTo;
                }
            }
        }

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

            return new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0, now.Offset);

        }
    }
}
