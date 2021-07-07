using FluentValidation;
using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    public class AddressWrapper : ModelWrapper<Address>
    {

        public AddressWrapper(Address model, IValidator<Address> validator = null) : base(model, validator)
        {
        }

        public int Id { get { return Model.Id; } }

        public string City
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string CityOriginalValue => GetOriginalValue<string>(nameof(City));

        public bool CityIsChanged => GetIsChanged(nameof(City));

        public string Street
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string StreetOriginalValue => GetOriginalValue<string>(nameof(Street));

        public bool StreetIsChanged => GetIsChanged(nameof(Street));

        public string StreetNumber
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string StreetNumberOriginalValue => GetOriginalValue<string>(nameof(StreetNumber));

        public bool StreetNumberIsChanged => GetIsChanged(nameof(StreetNumber));
    }
}
