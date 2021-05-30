using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Data.Repositories;
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
        private readonly IMessageDialogService messageDialogService;
        private readonly IValidator<Meeting> meetingValidator;
        private MeetingWrapper meeting;
        private Friend selectedAddedFriend;
        private Friend selectedAvailableFriend;

        public MeetingDetailViewModel(IMeetingRepository meetingRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IValidator<Meeting> meetingValidator) : base(eventAggregator)
        {
            this.meetingRepository = meetingRepository;
            this.messageDialogService = messageDialogService;
            this.meetingValidator = meetingValidator;

            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();

            AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
        }

        private void OnAddFriendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;
            
            Meeting.Model.Friends.Add(friendToAdd);
            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd); 
           
            HasChanges = meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnAddFriendCanExecute()
        {
            return SelectedAvailableFriend != null;
        }

        private void OnRemoveFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;
            
            Meeting.Model.Friends.Remove(friendToRemove);
            AddedFriends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove); 
            
            HasChanges = meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnRemoveFriendCanExecute()
        {
            return SelectedAddedFriend != null;

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

        public Friend SelectedAddedFriend
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

        public Friend SelectedAvailableFriend
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

        public ObservableCollection<Friend> AddedFriends { get; set; }
        public ObservableCollection<Friend> AvailableFriends { get; set; }

        public ICommand AddFriendCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        public override async Task LoadAsync(int? id)
        {
            Meeting meeting = id.HasValue
                ? await meetingRepository.GetByIdAsync(id.Value)
                : await CreateNewMeetingAsync();

            InitializeMeetingWrapper(meeting);

            TriggerValidationIfNew(meeting);

            // Load the friends for the picklist.

            await SetupPickList();

        }

        private async Task SetupPickList()
        {
            
            
            List<Friend> allFriends = await meetingRepository.GetAllFriendsAsync();

            //Sort the list by Friend FullName
            allFriends = allFriends.OrderBy(f => f.FullName).ToList();

            // Clear the two public observable collections.
            AddedFriends.Clear();
            AvailableFriends.Clear();

            foreach (var friend in allFriends)
            {
                if (Meeting.Model.Friends.Contains(friend))
                {
                    AddedFriends.Add(friend);
                }
                else
                {
                    AvailableFriends.Add(friend);
                }
            }
        }

        protected override async void OnDeleteExecuteAsync()
        {
            MessageDialogResult result = messageDialogService.ShowOKCancelDialog(
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
                await meetingRepository.RemoveAsync(Meeting.Model);

                HasChanges = meetingRepository.HasChanges();

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
                && HasChanges;
        }

        protected override async void OnSaveExecuteAsync()
        {
            await meetingRepository.SaveAsync();

            HasChanges = meetingRepository.HasChanges();

            base.RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
        }
       
        private void InitializeMeetingWrapper(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting, meetingValidator);

            Meeting.PropertyChanged += (s, e) =>
            {
                // if no changes have been detected yet...
                if (!HasChanges)
                {
                    // Check to see if the entity in the repository has changed.
                    // So, once True, this will not be checked again until the entity is reloaded.
                    HasChanges = meetingRepository.HasChanges();
                }

                // The HasErrors property of the Entity has changed...
                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    // Raise the CanExecute changed event for the save command
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
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
