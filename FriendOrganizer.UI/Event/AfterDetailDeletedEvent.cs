using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    /// <summary>
    ///  Public event called after a subclass of <see cref="DetailViewModelBase"/> deletes an instance of a subclass of 
    ///  <see cref="EntityBase"/>.  Carries <see cref="AfterDetailDeletedEventArgs"/>
    /// </summary>
    public class AfterDetailDeletedEvent : PubSubEvent<AfterDetailDeletedEventArgs> { }

    /// <summary>
    /// Payload: Id (int) of the deleted <see cref="EntityBase"/> and the ViewModelName (string) that deleted it.
    /// </summary>
    public class AfterDetailDeletedEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
