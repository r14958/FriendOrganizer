using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    /// <summary>
    /// Public event called after an instance of a subclass of <see cref="DetailViewModelBase"/> showing an instance of a subclass of a 
    /// <see cref="EntityBase"/> is opened.  Carries <see cref="AfterDetailDeletedEventArgs"/>
    /// </summary>
    public class OpenDetailViewEvent : PubSubEvent<OpenDetailViewEventArgs> { }

    /// <summary>
    /// Payload: Id (int) of the <see cref="EntityBase"/> and ViewModelName (string) that was opened.
    /// </summary>
    public class OpenDetailViewEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}