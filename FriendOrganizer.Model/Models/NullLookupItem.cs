namespace FriendOrganizer.Domain.Models
{
    public class NullLookupItem<T> : LookupItem<T> where T: EntityBase
    {
        public new int? Id { get { return null; } }
    }
}
