using FriendOrganizer.UI.ViewModel;
using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI
{
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : NotifyPropChangedBase;
    public delegate IDetailViewModel CreateDetailViewModel(string viewModelName);

    public static class Utilities
    {
        public enum ViewType
        {
            FriendDetail,
            MeetingDetail,
            Navigation

        }

        public static async void Await(this Task task, Action onCompleted = null, Action<Exception> onError = null)
        {
            try
            {
                await task;
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {

                onError?.Invoke(e);
            }
        }

        
    }
}
