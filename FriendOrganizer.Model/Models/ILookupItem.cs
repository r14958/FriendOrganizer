namespace FriendOrganizer.Domain.Models
{
    public interface ILookupItem
    {
        string DisplayMember { get; set; }
        int Id { get; set; }
    }
}