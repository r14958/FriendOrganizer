using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.ViewModel.Factory;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    /// <summary>
    /// View model for the FriendDetail view.
    /// </summary>
    public class FriendDetailViewModel : ViewModelBase, IViewModel<Friend>
    {
        private readonly IDataRepository<Friend> friendRepository;
        private readonly IEventAggregator eventAggregator;
        private readonly IValidator<Friend> friendValidator;
        private readonly IMessageDialogService messageDialogService;
        private readonly IFriendOrganizerViewModelFactory viewModelFactory;
        private readonly ILookupDataService<ProgrammingLanguage> programmingLanguageLookupDataService;
        private IViewModel<FriendPhoneNumber> friendPhoneNumberListViewModel;
        private FriendWrapper friend;
        private bool hasChanges;
        private bool hasPhoneNumberError = false;

        /// <summary>
        /// Public constructor for the view model.
        /// </summary>
        /// <param name="friendRepository">Data service to provide a <see cref="Friend"/> instance.</param>
        /// <param name="friendValidator">FluentValidation <see cref="FluentValidation.IValidator{T}"/> to perform data validation on the <see cref="Friend"/> instance.</param>
        public FriendDetailViewModel(IDataRepository<Friend> friendRepository,
            IEventAggregator eventAggregator,
            IValidator<Friend> friendValidator,
            IMessageDialogService messageDialogService,
            IFriendOrganizerViewModelFactory viewModelFactory,
            ILookupDataService<ProgrammingLanguage> programmingLanguageLookupDataService)
        {
            this.friendRepository = friendRepository;
            this.eventAggregator = eventAggregator;
            this.friendValidator = friendValidator;
            this.messageDialogService = messageDialogService;
            this.viewModelFactory = viewModelFactory;
            this.programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            this.eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
               .Subscribe(OnOpenFriendDetailViewAsync);

            this.eventAggregator.GetEvent<PhoneNumberChangedEvent>()
                .Subscribe(OnPhoneNumberChanged);

            FriendPhoneNumberListViewModel = CreateFriendPhoneNumberListViewModel();
            
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem<ProgrammingLanguage>>();
        }

        private void OnPhoneNumberChanged(bool hasError)
        {
            hasPhoneNumberError = hasError;
            HasChanges = true;
        }

        private async void OnOpenFriendDetailViewAsync(int? friendId)
        {
            // If same Friend has been selected, do nothing..
            if (friendId == friend.Id) return;

            if (FriendPhoneNumberListViewModel !=null && FriendPhoneNumberListViewModel.HasChanges)
            {
                // Verify that the user really wants to navigate away 
                MessageDialogResult result = messageDialogService.ShowOKCancelDialog(
                    "You have made changes. Navigate away without saving?",
                    "Question");

                // If they cancel, do nothing.
                if (result == MessageDialogResult.Cancel) return;
            }

            FriendPhoneNumberListViewModel = CreateFriendPhoneNumberListViewModel();
            await FriendPhoneNumberListViewModel.LoadAsync(friendId);
        }

        private IViewModel<FriendPhoneNumber> CreateFriendPhoneNumberListViewModel()
        {
            return (IViewModel<FriendPhoneNumber>)viewModelFactory.CreateViewModel(ViewType.FriendPhoneNumberList);
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



        public IViewModel<FriendPhoneNumber> FriendPhoneNumberListViewModel
        {
            get { return friendPhoneNumberListViewModel; }
            set 
            { 
                friendPhoneNumberListViewModel = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<LookupItem<ProgrammingLanguage>> ProgrammingLanguages { get; }

        public bool HasChanges
        {
            get { return hasChanges; }
            set 
            {
                if (hasChanges != value)
                {
                    hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); 
                }
            }
        }

        /// <summary>
        /// Loads the target, wrapped <see cref="Friend"/> Entity from the repository.
        /// </summary>
        /// <param name="friendId">The Id of the <see cref="Friend"/> Entity to be loaded.</param>
        public async Task LoadAsync(int? friendId)
        {
            // Based on its Id, either get an existing Friend or create a new one.
            Friend friend = friendId.HasValue
                ? await friendRepository.GetAsync(friendId.Value)
                : await CreateNewFriendAsync();

            InitializeFriendWrapper(friend);

            TriggerValidationIfNew(friend);

            await LoadProgrammingLanguagesLookupAsync();

            await FriendPhoneNumberListViewModel.LoadAsync(friendId);
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

        
        public ICommand SaveCommand { get; }

        public ICommand DeleteCommand { get; }
        
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

        private bool OnSaveCanExecute()
        {
            // Can only execute if the wrapped Entity is not null, does not have errors,
            // and has been changed since loaded.
            return Friend != null && !Friend.HasErrors && HasChanges & !hasPhoneNumberError;
        }

        private async void OnSaveExecute()
        {
            // Use the data repository to save the friend to the DB.
            await friendRepository.SaveAsync();

            // Once the Friend entity has been saved to the DB, update
            // HasChanges property for the view model.  It should now be False.
            HasChanges = friendRepository.HasChanges();

            // Raise (publish) the AfterFriendSavedEvent, passing on the Id of the
            // updated friend, and its (perhaps changed) DisplayMember.
            eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(
                new AfterFriendSavedEventsArgs
                {
                    Id = Friend.Id,
                    DisplayMember = Friend.FullName
                });
        }

        private bool OnDeleteCanExecute()
        {
            return Friend != null && Friend.Id != 0;
        }
        
        private async void OnDeleteExecute()
        {
            // Verify that the user really wants to delete the Friend 
            MessageDialogResult result = messageDialogService.ShowOKCancelDialog(
                $"Do you really want to delete {Friend.FullName}?",
                "Question");

            // If they cancel, do nothing.
            if (result == MessageDialogResult.Cancel)
            {
                return;
            }
            else
            {
                await friendRepository.DeleteAsync(Friend.Model);

                HasChanges = friendRepository.HasChanges();

                eventAggregator.GetEvent<AfterFriendDeletedEvent>()
                    .Publish(Friend.Id); 
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
            await friendRepository.CreateAsync(friend);

            return friend;
        }

    }
}
