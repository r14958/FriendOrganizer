using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FriendOrganizer.UI.Utilities;

namespace FriendOrganizer.UI.ViewModel.Factory
{
    public class FriendOrganizerViewModelFactory : IFriendOrganizerViewModelFactory
    {
        private readonly CreateViewModel<FriendDetailViewModel> createFriendDetailViewModel;
        private readonly CreateViewModel<MeetingDetailViewModel> createMeetingDetailViewModel;
        private readonly CreateViewModel<NavigationViewModel> createNavigationViewModel;

        public FriendOrganizerViewModelFactory(
            CreateViewModel<FriendDetailViewModel> createFriendDetailViewModel,
            CreateViewModel<MeetingDetailViewModel> createMeetingDetailViewModel,
            CreateViewModel<NavigationViewModel> createNavigationViewModel)
        {
            this.createFriendDetailViewModel = createFriendDetailViewModel;
            this.createMeetingDetailViewModel = createMeetingDetailViewModel;
            this.createNavigationViewModel = createNavigationViewModel;
        }

        public ViewModelBase CreateViewModel(ViewType viewType)
        {
            return viewType switch
            {
                ViewType.FriendDetail => createFriendDetailViewModel(),
                ViewType.MeetingDetail => createMeetingDetailViewModel(),
                ViewType.Navigation => createNavigationViewModel(),
                _ => throw new ArgumentException("The ViewType does not have a ViewModel.", nameof(viewType)),
            };
        }
    }
}
