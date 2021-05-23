using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.Factory
{
    public class FriendOrganizerViewModelFactory : IFriendOrganizerViewModelFactory
    {
        private readonly CreateViewModel<FriendDetailViewModel> createFriendDetailViewModel;
        private readonly CreateViewModel<FriendPhoneNumberListViewModel> createFriendPhoneNumberListViewModel;
        private readonly CreateViewModel<NavigationViewModel> createNavigationViewModel;

        public FriendOrganizerViewModelFactory(
            CreateViewModel<FriendDetailViewModel> createFriendDetailViewModel,
            CreateViewModel<FriendPhoneNumberListViewModel> createFriendPhoneNumberListViewModel,
            CreateViewModel<NavigationViewModel> createNavigationViewModel)
        {
            this.createFriendDetailViewModel = createFriendDetailViewModel;
            this.createFriendPhoneNumberListViewModel = createFriendPhoneNumberListViewModel;
            this.createNavigationViewModel = createNavigationViewModel;
        }

        public ViewModelBase CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.FriendDetail:
                    return createFriendDetailViewModel();
                case ViewType.FriendPhoneNumberList:
                    return createFriendPhoneNumberListViewModel();
                case ViewType.Navigation:
                    return createNavigationViewModel();
                default:
                    throw new ArgumentException("The ViewType does not have a ViewModel.", nameof(viewType));
            }
        }
    }
}
