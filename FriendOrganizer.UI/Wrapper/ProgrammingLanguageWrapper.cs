using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    public class ProgrammingLanguageWrapper : ModelWrapper<ProgrammingLanguage>
    {
        public ProgrammingLanguageWrapper(ProgrammingLanguage model) 
            : base(model)
        {
        }

        public int Id { get { return Model.Id; } }

        public string Name
        {
            get { return GetValueOrDefault<string>(); }
            set { SetValue(value); }
        }

        public string NameOriginalValue => GetOriginalValue<string>(nameof(Name));

        public bool NameIsChanged => GetIsChanged(nameof(Name));
    }
}
