namespace FriendOrganizer.UI.Services
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowOKCancelDialog(string text, string title);
    }
}