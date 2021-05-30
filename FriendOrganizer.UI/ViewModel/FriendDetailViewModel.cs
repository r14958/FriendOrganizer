﻿using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.Validator;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    /// <summary>
    /// View model for the FriendDetail view.
    /// </summary>
    public class FriendDetailViewModel : DetailViewModelBase
    {
        private readonly IFriendRepository friendRepository;
        private readonly IValidator<Friend> friendValidator;
        private readonly IValidator<FriendPhoneNumber> phoneNumberValidator;
        private readonly IMessageDialogService messageDialogService;
        private readonly ILookupDataService<ProgrammingLanguage> programmingLanguageLookupDataService;
        private FriendPhoneNumberWrapper selectedPhoneNumber;
        private FriendWrapper friend;

        /// <summary>
        /// Public constructor for the view model.
        /// </summary>
        /// <param name="friendRepository">Data service to provide a <see cref="Friend"/> instance.</param>
        /// <param name="friendValidator">FluentValidation <see cref="FluentValidation.IValidator{T}"/> to perform data validation on the <see cref="Friend"/> instance.</param>
        public FriendDetailViewModel(IFriendRepository friendRepository,
            IEventAggregator eventAggregator,
            IValidator<Friend> friendValidator,
            IValidator<FriendPhoneNumber> phoneNumberValidator,
            IMessageDialogService messageDialogService,
            ILookupDataService<ProgrammingLanguage> programmingLanguageLookupDataService) : base(eventAggregator)
        {
            this.friendRepository = friendRepository;
            this.friendValidator = friendValidator;
            this.phoneNumberValidator = phoneNumberValidator;
            this.messageDialogService = messageDialogService;
            this.programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);


            ProgrammingLanguages = new ObservableCollection<LookupItem<ProgrammingLanguage>>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
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

        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        public ObservableCollection<LookupItem<ProgrammingLanguage>> ProgrammingLanguages { get; }

        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get { return selectedPhoneNumber; }
            set 
            { 
                selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }
            

        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }

        /// <summary>
        /// Loads the target, wrapped <see cref="Friend"/> Entity from the repository.
        /// </summary>
        /// <param name="friendId">The Id of the <see cref="Friend"/> Entity to be loaded.</param>
        public override async Task LoadAsync(int? friendId)
        {
            // Based on its Id, either get an existing Friend or create a new one.
            Friend friend = friendId.HasValue
                ? await friendRepository.GetByIdAsync(friendId.Value)
                : await CreateNewFriendAsync();

            InitializeFriendWrapper(friend);

            TriggerValidationIfNew(friend);

            InitializeFriendPhoneNumbers(friend);

            await LoadProgrammingLanguagesLookupAsync();
        }

        private void InitializeFriendPhoneNumbers(Friend friend)
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }

            PhoneNumbers.Clear();

            foreach (var phoneNumber in friend.PhoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(phoneNumber, phoneNumberValidator);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
                PhoneNumbers.Add(wrapper);
            }
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = friendRepository.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Initializes a <see cref="FriendOrganizer.Model.Friend"/> into a <see cref="FriendWrapper"/> <see cref="Friend"/>,
        /// including data validation and change detection.
        /// </summary>
        /// <param name="friend">The <see cref="FriendOrganizer.Model.Friend"/> that is being wrapped for the <see cref="FriendDetailViewModel"/>.</param>
        private void InitializeFriendWrapper(Friend friend)
        {
            Friend = new FriendWrapper(friend, friendValidator);

            // Register this method to run whenever a Friend property changes. It will not run 
            // during the Load.
            Friend.PropertyChanged += (s, e) =>
            {
                // if no changes have been detected yet...
                if (!HasChanges)
                {
                    // Check to see if the entity in the repository has changed.
                    // So, once True, this will not be checked again until the entity is reloaded.
                    HasChanges = friendRepository.HasChanges();
                }

                // The HasErrors property of the Entity has changed...
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    // Raise the CanExecute changed event for the save command
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };

            // Now that the entity has been loaded, raise the CanExecute changed event of the SaveCommand.
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

       
        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }

        private void OnRemovePhoneNumberExecute()
        {
            // Remove the wrapped phone number's event handler
            SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            // Remove the phone number entity from the wrapped Friend entity's collection.
            Friend.Model.PhoneNumbers.Remove(SelectedPhoneNumber.Model);
            // Remove the wrapped phone number from the public collection.
            PhoneNumbers.Remove(SelectedPhoneNumber);
            // Null out the wrapped phone entity.
            SelectedPhoneNumber = null;
            // Re-sync the view model's HasChanges with the repository.
            HasChanges = friendRepository.HasChanges();
            // Recheck the CanExecute property of the save command.
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

       

        private void OnAddPhoneNumberExecute()
        {
            // Create a new wrapper with a new (empty) phone number entity.
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber(), new FriendPhoneNumberValidator());
            // Add a property changed event handler to it.
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            // Add the wrapper to the public collection
            PhoneNumbers.Add(newNumber);
            // Add the FriendPhoneNumber entity to the PhoneNumbers collection of the wrapped Friend entity.
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            // Trigger Validation
            newNumber.Number = "";
        }

        /// <summary>
        /// Loads <see cref="LookupItem{T}"/> from the <see cref="ILookupDataService{T}"/> where T = <see cref="ProgrammingLanguage"/> into
        /// the observable collection <see cref="ProgrammingLanguages"/>.
        /// </summary>
        /// <returns>A <see cref="void"/> <see cref="Task"/></returns>
        private async Task LoadProgrammingLanguagesLookupAsync()
        {
            // Clear any existing entries, so they are not double loaded.
            ProgrammingLanguages.Clear();

            // Add a null lookup item with Id = null, so a user can "erase" a favorite language selection.
            // It will display a dash.
            ProgrammingLanguages.Add(new NullLookupItem<ProgrammingLanguage> { DisplayMember = " - " });

            // Load the real language selections from the LookupDataService
            var lookup = await programmingLanguageLookupDataService.GetLookupAsync();
            foreach (var lookupItem in lookup)
            {
                ProgrammingLanguages.Add(lookupItem);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            // Can only execute if the wrapped Entity is not null, does not have errors,
            // and has been changed since loaded.
            return Friend != null 
                && !Friend.HasErrors
                && PhoneNumbers.All(pn => !pn.HasErrors)
                && HasChanges;
        }

        protected override async void OnSaveExecuteAsync()
        {
            // Use the data repository to save the friend to the DB.
            await friendRepository.SaveAsync();

            // Once the Friend entity has been saved to the DB, update
            // HasChanges property for the view model.  It should now be False.
            HasChanges = friendRepository.HasChanges();

            // Raise (publish) the AfterFriendSavedEvent, passing on the Id of the
            // updated friend, and its (perhaps changed) DisplayMember.
            base.RaiseDetailSavedEvent(Friend.Id, Friend.FullName);
            
        }

        protected override bool OnDeleteCanExecute()
        {
            return Friend != null && Friend.Id != 0;
        }
        
        protected override async void OnDeleteExecuteAsync()
        {
            // If the targeted friend has been added to any meeting, do not allow the deletion.
            if (await friendRepository.HasMeetingsAsync(friend.Id))
            {
                messageDialogService.ShowInfoDialog($"{Friend.FullName} cannot be deleted, as this friend is part of at least one meeting.");
                return;
            }
            
            // Verify that the user really wants to delete the Friend 
            MessageDialogResult result = messageDialogService.ShowOKCancelDialog(
                $"Do you really want to delete {Friend.FullName}?",
                "Question");

            // If they approve...
            if (result == MessageDialogResult.OK)
            {
                await friendRepository.RemoveAsync(Friend.Model);

                HasChanges = friendRepository.HasChanges();

                base.RaiseDetailDeletedEvent(Friend.Id);
            }
        }

        /// <summary>
        /// If the <see cref="Friend"/> entity was just created, trigger validation to help the user fill out the required information.
        /// </summary>
        /// <param name="friend">The <see cref="Friend"/> entity being edited or created.</param>
        private void TriggerValidationIfNew(Friend friend)
        {
            // If the friend has not yet been saved to the DB...
            if (friend.Id == 0)
            {
                // Trick to trigger validation to show user what needs to be filled out.
                Friend.FirstName = string.Empty;
            }
        }

        private async Task<Friend> CreateNewFriendAsync()
        {
            // Create a new Friend entity.
            Friend friend = new();
            
            // Add the new Friend to the friend data repository.
            await friendRepository.AddAsync(friend);

            return friend;
        }

    }
}
