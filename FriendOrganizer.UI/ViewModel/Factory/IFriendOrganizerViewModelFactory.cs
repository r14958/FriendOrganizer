using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.Factory
{
    public enum ViewType
    {
        FriendDetail,
        FriendPhoneNumberList,
        Navigation

    }
    public interface IFriendOrganizerViewModelFactory
    {
        ViewModelBase CreateViewModel(ViewType viewType);
    }
}
