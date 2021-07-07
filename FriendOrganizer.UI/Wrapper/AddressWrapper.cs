using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    public class AddressWrapper : ModelWrapper<Address>
    {
        private const int MinimumCityLength = 2;
        private const int MaximumCityLength = 50;
        private const int MaximumStreetLength = 50;
        private const int MaximumStreetNumberLength = 20;

        public AddressWrapper(Address model) : base(model)
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
            set { SetValue(value.Trim()); }
        }

        public string StreetNumberOriginalValue => GetOriginalValue<string>(nameof(StreetNumber));

        public bool StreetNumberIsChanged => GetIsChanged(nameof(StreetNumber));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(City))
            {
                yield return new ValidationResult("Please specify a city name.",
                    new[] { nameof(City) });
            }
            if (City == null || City.Length < MinimumCityLength)
            {
                yield return new ValidationResult($"City name must be at least {MinimumCityLength} characters.",
                    new[] { nameof(City) });
            }
            if (City != null && City.Length > MaximumCityLength)
            {
                yield return new ValidationResult($"City name cannot exceed {MaximumCityLength} characters.",
                                   new[] { nameof(City) });
            }
            if (Street !=null && Street.Length > MaximumStreetLength)
            {
                yield return new ValidationResult($"Street name cannot exceed {MaximumStreetLength} characters.",
                                   new[] { nameof(Street) });
            }
            if (StreetNumber != null && StreetNumber.Length > MaximumStreetNumberLength)
            {
                yield return new ValidationResult($"Street number cannot exceed {MaximumStreetNumberLength} characters.",
                                   new[] { nameof(StreetNumber) });
            }

        }
    }
}
