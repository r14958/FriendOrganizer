using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    /// <summary>
    /// Public event called after a subclass of <see cref="DetailViewModelBase"/> showing an instance of a subclass of 
    /// <see cref="EntityBase"/> is closed.  Carries <see cref="AfterDetailCloseEventArgs"/>
    /// </summary>
    public class AfterDetailCloseEvent : PubSubEvent<AfterDetailCloseEventArgs> { }

    /// <summary>
    /// Payload: Id (int) of the <see cref="EntityBase"/> and the ViewModelName (string) that was closed.
    /// </summary>
    public class AfterDetailCloseEventArgs
    {
    public int Id { get; set; }
    public string ViewModelName { get; set; }
    }
    
}
