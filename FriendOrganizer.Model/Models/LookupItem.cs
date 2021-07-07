using System.Diagnostics;

namespace FriendOrganizer.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Display: {DisplayMember)")]
    public class LookupItem<T> : EntityBase, ILookupItem where T : EntityBase
    {
        public string DisplayMember { get; set; }
    }
}
