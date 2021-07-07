using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Commands;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
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
    public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendRepository friendRepository;
        private readonly ILookupDataService<ProgrammingLanguage> programmingLanguageLookupDataService;
        private FriendPhoneNumberWrapper selectedPhoneNumber;
        private FriendWrapper friend;

        /// <summary>
        /// Public constructor for the view model.
        /// </summary>
        /// <param name="friendRepository">Data service to provide a <see cref="Friend"/> instance.</param>
        /// <param name="friendValidator">FluentValidation <see cref="FluentValidation.IValidator{T}"/> to perform data validation on the <see cref="Friend"/> instance.</param>
        public FriendDetailViewModel(
            IFriendRepository friendRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            ILookupDataService<ProgrammingLanguage> programmingLanguageLookupDataService) : base(eventAggregator, messageDialogService)
        {
            this.friendRepository = friendRepository;
            this.programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

            eventAggregator.GetEvent<AfterCollectionSavedEvent>().Subscribe(AfterCollectionSaved);

            ProgrammingLanguages = new ObservableCollection<LookupItem<ProgrammingLanguage>>();
        }

        /// <summary>
        /// Gets and sets the properties of an instance of the wrapped entity of <see cref="Domain.Models.Friend"/>.
        /// </summary>
        public FriendWrapper Friend
        {
            get { return friend; }
            private set
            {
                friend = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="ProgrammingLanguage"/>.
        /// </summary>
        public ObservableCollection<LookupItem<ProgrammingLanguage>> ProgrammingLanguages 
        { 
            get; 
        }

        /// Gets and sets the properties of an instance of the wrapped entity of <see cref="FriendPhoneNumber"/>.
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
        public override async Task LoadAsync(int friendId)
        {
            // Based on its Id, either get an existing Friend or create a new one.
            Friend friend = friendId > 0
                ? await friendRepository.GetByIdAsync(friendId)
                : await CreateNewFriendAsync();

            // Set the ViewModel Id property.
            Id = friend.Id;

            InitializeFriendWrapper(friend);

            // Set the Title property of the ViewModel.
            SetTitle();

            TriggerValidationIfNew(friend);

            await LoadProgrammingLanguagesLookupAsync();
        }
        
        protected override bool OnSaveCanExecute()
        {
            // Can only execute if the wrapped Entity is not null, does not have errors,
            // and has been changed since loaded.
            var notNull = Friend != null;
            var isValid = Friend.IsValid;
            var isChanged = Friend.IsChanged;
            return notNull
                && isValid
                && isChanged;
        }

        protected override bool OnSaveAllDetailCanExecute()
        {
            return NotifyDataErrorInfoBase.HasAnyChanges;
        }

        protected override async void OnSaveExecuteAsync()
        {
            await SaveWithOptimisticConcurrencyAsync(friendRepository.SaveAsync,
                () =>
                {
                    //Resync the VM's Id to the Friend's Id.
                    Id = Friend.Id;

                    SetTitle();

                    Friend.AcceptChanges();

                    // Raise (publish) the AfterFriendSavedEvent, passing on the Id of the
                    // updated friend, and its (perhaps changed) DisplayMember.
                    base.RaiseDetailSavedEvent(Friend.Id, Friend.FullName);
                });
        }

        protected override bool OnDeleteCanExecute()
        {
            return Friend != null;
        }

        protected override async void OnDeleteExecuteAsync()
        {
            // If the targeted friend has been added to any meeting, do not allow the deletion.
            if (await friendRepository.HasMeetingsAsync(friend.Id))
            {
                await messageDialogService.ShowInfoDialogAsync($"{Friend.FullName} cannot be deleted, as this friend is part of at least one meeting.");
                return;
            }

            // Verify that the user really wants to delete the Friend 
            var result = await messageDialogService.ShowOKCancelDialogAsync(
                $"Do you really want to delete {Friend.FullName}?",
                "Question");

            // If they approve...
            if (result == MessageDialogResult.OK)
            {
                friendRepository.Remove(Friend.Model);

                await friendRepository.SaveAsync();

                base.RaiseDetailDeletedEvent(Friend.Id);
            }
        }

        protected override bool OnResetCanExecute()
        {
            return Friend.IsChanged || Friend.PhoneNumbers.Any(pn => pn.IsChanged);
        }

        protected override void OnResetExecuteAsync()
        {
            Friend.RejectChanges();
            SetTitle();
            Friend.FirstName += string.Empty;
        }

        private async Task<Friend> CreateNewFriendAsync()
        {
            // Create a new Friend entity.
            Friend friend = new();

            // Add the new Friend to the friend data repository.
            await friendRepository.AddAsync(friend);

            return friend;
        }

        /// <summary>
        /// Initializes a <see cref="FriendOrganizer.Model.Friend"/> into a <see cref="Wrapper.FriendWrapper"/> <see cref="Friend"/>,
        /// including data validation and change detection.
        /// </summary>
        /// <param name="friend">The <see cref="FriendOrganizer.Model.Friend"/> that is being wrapped for the <see cref="FriendDetailViewModel"/>.</param>
        private void InitializeFriendWrapper(Friend friend)
        {
            Friend = new FriendWrapper(friend);

            // Register this method to run whenever a Friend property changes. It will not run 
            // during the Load.
            Friend.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Friend.IsChanged)
                || e.PropertyName == nameof(Friend.IsValid))
                {
                    HasChanges = Friend.IsChanged;
                    base.InvalidateControls();
                }

                // Update VM Title if either the Friend's FirstName or LastName has changed.
                if (e.PropertyName == nameof(Friend.FirstName) 
                    || e.PropertyName == nameof(Friend.LastName))
                {
                    SetTitle();
                }

                //// The HasErrors property of the Entity has changed...
                //if (e.PropertyName == nameof(Friend.HasErrors))
                //{
                //    // Raise the CanExecute changed event for all inherited delegate commands.
                //    base.InvalidateControls();
                //}
            };

            // Now that the entity has been loaded, raise the CanExecute changed event of the SaveCommand.
            base.InvalidateControls();
        }
        
        private void TriggerValidationIfNew(Friend friend)
        {
            // If the friend has not yet been saved to the DB...
            if (friend.Id == 0)
            {
                // Trick to trigger validation to show user what needs to be filled out.
                Friend.FirstName = string.Empty;

                // Tell the FriendWrapper's change tracker to ignore the above change,
                // so the new, blank wrapper is not treated as if it has been changed by the user.
                Friend.IgnoreChange(nameof(Friend.FirstName));
            }
            SetTitle();
        }

        private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            // If it was a ProgrammingLanguageDetailViewModel that was saved...
            if (args.ViewModelName == nameof(ProgrammingLanguageDetailViewModel))
            {
                // Update the meeting repository with the updated Friend info.
                await LoadProgrammingLanguagesLookupAsync();
            }
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                base.InvalidateControls();
                ((DelegateCommand)AddPhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }

        private void SetTitle()
        {
            Title = Friend.FullName;
        }

        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }

        private void OnRemovePhoneNumberExecute()
        {
            Friend.PhoneNumbers.Remove(SelectedPhoneNumber);
            ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPhoneNumberExecute()
        {
            // Create a new wrapper with a new (empty) phone number entity.
            var newNumberWrapper = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            
            // Add the empty phone number wrapper to the collection in the Friend wrapper.
            Friend.PhoneNumbers.Add(newNumberWrapper);

            // Trigger Validation
            newNumberWrapper.Number = "";
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
    }
}
