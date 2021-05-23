using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.ViewModel.Factory;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IMessageDialogService messageDialogService;
        private readonly IFriendOrganizerViewModelFactory viewModelFactory;
        private IViewModel<Friend> friendDetailViewModel;

        public MainViewModel(
            IFriendOrganizerViewModelFactory viewModelFactory, 
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
        {
            this.viewModelFactory = viewModelFactory;
            this.eventAggregator = eventAggregator;
            this.messageDialogService = messageDialogService;

            // When a new FriendDetailView is opened, call the appropriate method to populate it.
            this.eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);
            this.eventAggregator.GetEvent<AfterFriendDeletedEvent>()
                .Subscribe(AfterFriendDeleted);

            NavigationViewModel = (NavigationViewModel)viewModelFactory.CreateViewModel(ViewType.Navigation);

            CreateNewFriendCommand = new DelegateCommand(OnCreateNewFriend);

        }

        
        /// <summary>
        /// Gets the <see cref="ViewModel.NavigationViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Set in the class's constructor, so no setter is needed.</remarks>
        public INavigationViewModel NavigationViewModel { get; }


        /// <summary>
        /// Gets the <see cref="ViewModel.FriendDetailViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Private setter, since this is only set during the <see cref="OnOpenFriendDetailViewAsync(int)"/> event.</remarks>
        public IViewModel<Friend> FriendDetailViewModel
        {
            get { return friendDetailViewModel; }
            private set 
            { 
                friendDetailViewModel = value;
                OnPropertyChanged();

            }
        }

        

        /// <summary>
        /// L
        /// </summary>
        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewFriendCommand { get; }

        /// <summary>
        /// Creates a new <see cref="FriendDetailViewModel"/> and populates it. 
        /// </summary>
        /// <param name="friendId">The ID of the target friend, or null to create a new Friend.</param>
        /// <remarks>Will check for changes to the existing <see cref="FriendDetailViewModel"/> and verify with the user before 
        /// navigating away without saving.</remarks>
        private async void OnOpenFriendDetailView(int? friendId)
        {
            // If we already have a view model and it has changes...
            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges)
            {
                // Verify that the user really wants to navigate away 
                MessageDialogResult result = messageDialogService.ShowOKCancelDialog(
                    "You have made changes. Navigate away without saving?",
                    "Question");

                // If they cancel, do nothing.
                if (result == MessageDialogResult.Cancel) return;
            }

            // Otherwise create a new FriendDetailViewModel and load the data of the selected Friend.
            FriendDetailViewModel = (IViewModel<Friend>)viewModelFactory.CreateViewModel(ViewType.FriendDetail);
            await FriendDetailViewModel.LoadAsync(friendId);
        }

        private void OnCreateNewFriend()
        {
            OnOpenFriendDetailView(null);
        }

        private void AfterFriendDeleted(int friendId)
        {
            FriendDetailViewModel = null;
        }

    }
}
