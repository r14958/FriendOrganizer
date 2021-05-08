using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    public class AfterFriendSavedEvent : PubSubEvent<AfterFriendSavedEventsArgs>
    {
    }

    public class AfterFriendSavedEventsArgs
    {
        public int Id { get; set; }

        public string DisplayMember { get; set; }
    }
}
