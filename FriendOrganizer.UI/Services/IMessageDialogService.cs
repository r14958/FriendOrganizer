using System.Threading.Tasks;

namespace FriendOrganizer.UI.Services
{
    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowOKCancelDialogAsync(string title, string text);

        public Task ShowInfoDialogAsync(string text);
    }
}