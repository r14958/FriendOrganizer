using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Validator;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    /// <summary>
    /// View model for the FriendDetail view.
    /// </summary>
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IDataServiceAsync<Friend> friendDataService;
        private readonly IEventAggregator eventAggregator;
        private readonly IValidator<Friend> validator;
        private FriendWrapper friend;

        /// <summary>
        /// Public constructor for the view model.
        /// </summary>
        /// <param name="friendDataService">Data service to provide a <see cref="Friend"/> instance.</param>
        /// <param name="eventAggregator">Prism <see cref="Prism.Events.EventAggregator"/> to pass events between view models.</param>
        /// <param name="validator">FluentValidation <see cref="FluentValidation.IValidator{T}"/> to perform data validation on the <see cref="Friend"/> instance.</param>
        public FriendDetailViewModel(IDataServiceAsync<Friend> friendDataService,
            IEventAggregator eventAggregator, IValidator<Friend> validator)
        {
            this.friendDataService = friendDataService;
            this.eventAggregator = eventAggregator;
            this.validator = validator;

            // Subscribe to the OpenFriendDetailViewEvent, calling OnOpenFriendDetailViewAsync if it occurs.
            this.eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailViewAsync);

            SaveCommand = new DelegateCommand(OnSaveExecuteAsync, OnSaveCanExecute);
        }
        public FriendWrapper Friend
        {
            get { return friend; }
            private set
            {
                friend = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync(int friendId)
        {
            Friend friend = await friendDataService.GetAsync(friendId);
            Friend = new FriendWrapper(friend, validator);

            Friend.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute()
        {
            //TODO: Check in addition if friend has changes.
            return Friend != null && !Friend.HasErrors;
        }

        private async void OnSaveExecuteAsync()
        {
            // Use the data service to save the friend to the DB.
            await friendDataService.UpdateAsync(Friend.Model);
            eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(
                new AfterFriendSavedEventsArgs
                {
                    Id = Friend.Id,
                    DisplayMember = Friend.FullName
                });
        }

        private async void OnOpenFriendDetailViewAsync(int friendId)
        {
            await LoadAsync(friendId);
        }
    }
}
