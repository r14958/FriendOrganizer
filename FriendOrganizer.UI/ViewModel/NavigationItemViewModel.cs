using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string displayMember;

        public NavigationItemViewModel(int Id, string displayMember)
        {
            this.Id = Id;
            this.displayMember = displayMember;
        }

        public int Id { get; }

        public string DisplayMember
        {
            get
            {
                return displayMember;
            }

            set
            {
                if (displayMember != value)
                {
                    displayMember = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
