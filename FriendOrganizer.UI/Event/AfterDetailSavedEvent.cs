using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    public class AfterDetailSavedEvent : PubSubEvent<AfterDetailSavedEventsArgs>
    {
    }

    public class AfterDetailSavedEventsArgs
    {
        public int Id { get; set; }

        public string DisplayMember { get; set; }

        public string ViewModelName { get; set; }
    }
}

