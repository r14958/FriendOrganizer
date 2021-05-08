namespace FriendOrganizer.Domain.Models
{
    public class LookupItem<T> : EntityBase where T : EntityBase
    {
        public string DisplayMember { get; set; }
    }
}
