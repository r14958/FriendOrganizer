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
using static FriendOrganizer.UI.Utilities;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IMessageDialogService messageDialogService;
        private readonly IFriendOrganizerViewModelFactory viewModelFactory;
        private IDetailViewModel detailViewModel;

        public MainViewModel(
            IFriendOrganizerViewModelFactory viewModelFactory, 
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
        {
            this.viewModelFactory = viewModelFactory;
            this.eventAggregator = eventAggregator;
            this.messageDialogService = messageDialogService;

            // When a new FriendDetailView is opened, call the appropriate method to populate it.
            this.eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);
            this.eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);

            NavigationViewModel = (NavigationViewModel)viewModelFactory.CreateViewModel(ViewType.Navigation);

            // Note that we are using a DelegateCommand<T>, where T is the type of the command parameter.
            // In this case, our parameter is of type Type, and is relaying the type of ViewModel to load.
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);

        }

        
        /// <summary>
        /// Gets the <see cref="ViewModel.NavigationViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Set in the class's constructor, so no setter is needed.</remarks>
        public INavigationViewModel NavigationViewModel { get; }


        /// <summary>
        /// Gets the <see cref="ViewModel.IDetailViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Private setter, since this is only set during the <see cref="OnOpenDetailView(OpenDetailViewEventArgs)"/> event.</remarks>
        public IDetailViewModel DetailViewModel
        {
            get { return detailViewModel; }
            private set 
            { 
                detailViewModel = value;
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

        public ICommand CreateNewDetailCommand { get; }

        /// <summary>
        /// Creates a new <see cref="DetailViewModel"/> and populates it. 
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>Will check for changes to the existing <see cref="DetailViewModel"/> and verify with the user before 
        /// navigating away without saving.</remarks>
        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            // If we already have a view model and it has changes...
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                // Verify that the user really wants to navigate away 
                MessageDialogResult result = messageDialogService.ShowOKCancelDialog(
                    "You have made changes. Navigate away without saving?",
                    "Question");

                // If they cancel, do nothing.
                if (result == MessageDialogResult.Cancel) return;
            }

            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    DetailViewModel = (IDetailViewModel)viewModelFactory.CreateViewModel(ViewType.FriendDetail);
                    break;
                case nameof(MeetingDetailViewModel):
                    DetailViewModel = (IDetailViewModel)viewModelFactory.CreateViewModel(ViewType.MeetingDetail);
                    break;
                default:
                    throw new ArgumentException($"ViewModel {args.ViewModelName} is unknown and cannot be opened.");
            }
            await DetailViewModel.LoadAsync(args.Id);
        }

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs { ViewModelName = viewModelType.Name });
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            DetailViewModel = null;
        }

    }
}
