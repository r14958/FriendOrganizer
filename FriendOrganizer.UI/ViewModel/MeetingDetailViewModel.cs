using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Commands;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase
    {
        private readonly IMeetingRepository meetingRepository;
        private readonly ILookupDataService<Friend> friendLookupDataService;
        private MeetingWrapper meeting;
        private FriendWrapper selectedAddedFriend;
        private FriendWrapper selectedAvailableFriend;

        public MeetingDetailViewModel(IMeetingRepository meetingRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            ILookupDataService<Friend> friendLookupDataService) : base(eventAggregator, messageDialogService)
        {
            this.meetingRepository = meetingRepository;
            this.friendLookupDataService = friendLookupDataService;
            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(OnDetailSaved);
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(OnDetailDeleted);


            AddedFriends = new ObservableCollection<FriendWrapper>();
            AvailableFriends = new ObservableCollection<FriendWrapper>();

            AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
        }

        public MeetingWrapper Meeting
        {
            get { return meeting; }
            private set
            {
                meeting = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FriendWrapper> AddedFriends { get; set; }
        public ObservableCollection<FriendWrapper> AvailableFriends { get; set; }

        public FriendWrapper SelectedAddedFriend
        {
            get { return selectedAddedFriend; }
            set
            {
                selectedAddedFriend = value;
                OnPropertyChanged();
                // When an AddedFriend is selected, check to see if it can be removed.
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public FriendWrapper SelectedAvailableFriend
        {
            get { return selectedAvailableFriend; }
            set
            {
                selectedAvailableFriend = value;
                OnPropertyChanged();
                // When an AvalaibleFriend is selected, check to see if it can be added.
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddFriendCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        public override async Task LoadAsync(int id)
        {
            Meeting meeting = id > 0
                ? await meetingRepository.GetByIdAsync(id)
                : await CreateNewMeetingAsync();

            // Set the ViewModel's Id to the Meeting's Id.
            Id = meeting.Id;

            InitializeMeetingWrapper(meeting);

            // Set the Title property of the ViewModel.
            SetTitle();

            TriggerValidationIfNew(meeting);

            await InitializeFriendPickListAsync(meeting);

            // Load the friends for the picklist.
            //await SetupPickList();

        }

        private async Task InitializeFriendPickListAsync(Meeting meeting)
        {
            foreach (var addedFriend in AddedFriends)
            {
                addedFriend.PropertyChanged -= AddedFriendPropertyChanged;
            }
           
            foreach (var availableFriend in AvailableFriends)
            {
                availableFriend.PropertyChanged -= AvailableFriendPropertyChanged;
            }

            AddedFriends.Clear();
            AvailableFriends.Clear();


            List<Friend> allFriends = await meetingRepository.GetAllFriendsAsync();

            //Sort the list by Friend FullName
            allFriends = allFriends.OrderBy(f => f.FullName).ToList();

            foreach (var friend in allFriends)
            {
                var wrapper = new FriendWrapper(friend);
                if (meeting.Friends.Contains(friend))
                {
                    wrapper.PropertyChanged += AddedFriendPropertyChanged;
                    AddedFriends.Add(wrapper);
                }
                else
                {
                    wrapper.PropertyChanged += AvailableFriendPropertyChanged;
                    AvailableFriends.Add(wrapper);
                }
            }
        }

        private void AddedFriendPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FriendWrapper.IsChanged)  || e.PropertyName == nameof(FriendWrapper.HasErrors))
            {
                base.InvalidateControls();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        private void AvailableFriendPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FriendWrapper.IsChanged) || e.PropertyName == nameof(FriendWrapper.HasErrors))
            {
                base.InvalidateControls();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }

        protected async override void OnDeleteExecuteAsync()
        {
            MessageDialogResult result = await messageDialogService.ShowOKCancelDialogAsync(
                $"Do you really want to delete {Meeting.Title}?",
                "Question");

            // If they cancel...
            if (result == MessageDialogResult.Cancel)
            {
                // ...do nothing.
                return;
            }
            else
            {
                meetingRepository.Remove(Meeting.Model);

                await meetingRepository.SaveAsync();

                //HasChanges = meetingRepository.HasChanges();

                base.RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        protected override bool OnDeleteCanExecute()
        {
            return Meeting != null;
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null
                && !Meeting.HasErrors
                && Meeting.IsChanged;
        }

        protected override async void OnSaveExecuteAsync()
        {
            await SaveWithOptimisticConcurrencyAsync(meetingRepository.SaveAsync,
               () =>
               {
                   //Resync the ViewModel's ID to the meeting's ID.
                   Id = Meeting.Id;

                   SetTitle();

                   Meeting.AcceptChanges();

                   base.RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
               });
        }

        protected override async void OnResetExecuteAsync()
        {
            Meeting.RejectChanges();
            SetTitle();
            Meeting.Title += string.Empty;
            await InitializeFriendPickListAsync(Meeting.Model);
            base.InvalidateControls();
        }

        protected override bool OnResetCanExecute()
        {
            return Meeting.IsChanged;
        }

        private async void OnDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            // If it was a FriendDetailViewModel that was deleted...
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                // Reload the picklist of Friends.
                await SetupPickList();
            }
        }

        /// <summary>
        /// Makes sure the picklist of Friends for the meeting contains the latest values.
        /// </summary>
        /// <param name="args"></param>
        private async void OnDetailSaved(AfterDetailSavedEventsArgs args)
        {
            // If it was a FriendDetailViewModel that was changed...
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                // Update the meeting repository with the updated Friend info.
                await meetingRepository.ReloadFriendAsync(args.Id);

                // Reload the picklist of Friends.
                await SetupPickList();
            }
        }

        private void OnAddFriendExecute()
        {
            
            //Meeting.Model.Friends.Add(SelectedAvailableFriend.Model);
            Meeting.AddedFriends.Add(SelectedAvailableFriend);
            AvailableFriends.Remove(SelectedAvailableFriend); 
           
            base.InvalidateControls();
        }

        private bool OnAddFriendCanExecute()
        {
            return SelectedAvailableFriend != null;
        }

        private void OnRemoveFriendExecute()
        {
            AvailableFriends.Add(SelectedAddedFriend);
            Meeting.AddedFriends.Remove(SelectedAddedFriend);

            base.InvalidateControls();
        }

        private bool OnRemoveFriendCanExecute()
        {
            return SelectedAddedFriend != null;

        }

        private void SetTitle()
        {
            Title = Meeting.Title;
        }

        private async Task SetupPickList()
        {
            
            
            List<Friend> allFriends = await meetingRepository.GetAllFriendsAsync();

            //Sort the list by Friend FullName
            var allFriendWrappers = allFriends.OrderBy(f => f.FullName).Select(f => new FriendWrapper(f));

            // Clear the two public observable collections.
            AddedFriends.Clear();
            AvailableFriends.Clear();


            foreach (var friendWrapper in allFriendWrappers)
            {
                if (Meeting.Model.Friends.Contains(friendWrapper.Model))
                {
                    AddedFriends.Add(friendWrapper);
                }
                else
                {
                    AvailableFriends.Add(friendWrapper);
                }
            }
        }
       
        private void InitializeMeetingWrapper(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);

            Meeting.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Meeting.IsChanged))
                {
                    HasChanges = Meeting.IsChanged;
                    base.InvalidateControls();
                }
                
                // Update VM Title if the Meeting's Title has changed.
                if (e.PropertyName == nameof(Meeting.Title))
                {
                    SetTitle();
                }

                // The HasErrors property of the Entity has changed...
                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    base.InvalidateControls();
                }
            };


            // Now that the entity has been loaded, raise the CanExecute changed event of the SaveCommand.
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

        private void TriggerValidationIfNew(Meeting meeting)
        {
            // If the entity has not yet been saved to the DB...
            if (meeting.Id == 0)
            {
                // Trick to trigger validation to show user what needs to be filled out.
                Meeting.Title = string.Empty;

                // Tell the MeetingWrapper's change tracker to ignore the above change,
                // so the new, blank wrapper is not treated as if it has been changed by the user.
                Meeting.IgnoreChange(nameof(Meeting.Title));
            }
        }

        private async Task<Meeting> CreateNewMeetingAsync()
        {
            Meeting meeting = new();

            await meetingRepository.AddAsync(meeting);

            return meeting;
        }
    }
}
