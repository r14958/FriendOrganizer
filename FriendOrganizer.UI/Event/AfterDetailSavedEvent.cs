using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    /// <summary>
    /// Public event called after a subclass of <see cref="DetailViewModelBase"/> saves an instance of a subclass of 
    /// <see cref="EntityBase"/>.  Carries <see cref="AfterDetailDeletedEventArgs"/>
    /// </summary>
    public class AfterDetailSavedEvent : PubSubEvent<AfterDetailSavedEventsArgs> { }

    /// <summary>
    ///  Payload: Id (int) and "DisplayMember" of the saved <see cref="EntityBase"/> and the ViewModelName (string) that saved it.
    /// </summary>
    public class AfterDetailSavedEventsArgs
    {
        public int Id { get; set; }

        public string DisplayMember { get; set; }

        public string ViewModelName { get; set; }
    }
}

