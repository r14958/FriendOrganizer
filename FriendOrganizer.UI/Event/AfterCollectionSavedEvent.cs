using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    /// <summary>
    /// Public event called after a subclass of <see cref="NotifyPropChangedBase"/> saves a collection of a subclass of 
    /// <see cref="EntityBase"/>.  Carries <see cref="AfterCollectionSavedEventArgs"/>
    /// </summary>
    public class AfterCollectionSavedEvent : PubSubEvent<AfterCollectionSavedEventArgs> { }

    
    /// <summary>
    /// Payload: ViewModelName (string) which saved the collection.
    /// </summary>
    public class AfterCollectionSavedEventArgs
    {
        public string ViewModelName { get; set; }
    }
}
