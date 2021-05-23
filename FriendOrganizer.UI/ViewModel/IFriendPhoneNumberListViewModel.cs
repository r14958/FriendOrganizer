using FriendOrganizer.UI.Wrapper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public interface IFriendPhoneNumberListViewModel
    {
        bool HasChanges { get; set; }

        Task LoadAsync(int? id);
    }
}