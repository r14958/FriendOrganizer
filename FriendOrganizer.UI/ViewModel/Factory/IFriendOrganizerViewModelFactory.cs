using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FriendOrganizer.UI.Utilities;

namespace FriendOrganizer.UI.ViewModel.Factory
{
    
    public interface IFriendOrganizerViewModelFactory
    {
        NotifyPropChangedBase CreateViewModel(ViewType viewType);
    }
}
