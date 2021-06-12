using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.Validator;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        private readonly IProgrammingLanguageRepository programmingLanguageRepository;
        private readonly IValidator<ProgrammingLanguage> languageValidator;
        private ProgrammingLanguageWrapper selectedProgrammingLanguage;

        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IProgrammingLanguageRepository programmingLanguageRepository,
            IValidator<ProgrammingLanguage> languageValidator,
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Programming Languages";
            this.programmingLanguageRepository = programmingLanguageRepository;
            this.languageValidator = languageValidator;

            ProgrammingLanguages = new ObservableCollection<ProgrammingLanguageWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }


        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; }


        public ProgrammingLanguageWrapper SelectedProgrammingLanguage
        {
            get { return selectedProgrammingLanguage; }
            set 
            {
                selectedProgrammingLanguage = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        
        private bool OnRemoveCanExecute()
        {
            return SelectedProgrammingLanguage != null;
        }

        private void OnAddExecute()
        {
            // Create a new wrapper with a new (empty) entity.
            var wrapper = new ProgrammingLanguageWrapper(new ProgrammingLanguage(), new ProgrammingLanguageValidator());
            // Add a property changed event handler to the wrapped entity.
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            // Add the new entity to the DbContext of the repository.
            programmingLanguageRepository.AddAsync(wrapper.Model);
            // Add the wrapper to the public collection
            ProgrammingLanguages.Add(wrapper);
            // Trigger Validation
            wrapper.Name = "";
        }

        private async void OnRemoveExecute()
        {
            var isReferenced =
                await programmingLanguageRepository.IsReferencedByFriendAync(SelectedProgrammingLanguage.Id);

            if (isReferenced)
            {
                await messageDialogService.ShowInfoDialogAsync($"The language {SelectedProgrammingLanguage.Name} " +
                    $"cannot be removed, as it is referenced by at least one friend.");
                return;
            }
            
            // Remove the wrapped entity's event handler
            SelectedProgrammingLanguage.PropertyChanged -= Wrapper_PropertyChanged;
            // Instruct the repository to remove the entity from it DbContext.
            programmingLanguageRepository.Remove(SelectedProgrammingLanguage.Model);
            // Remove the wrapped entity from the public collection.
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage);
            // Null out the wrapped entity.
            SelectedProgrammingLanguage = null;
            // Re-sync the view model's HasChanges with the repository.
            HasChanges = programmingLanguageRepository.HasChanges();
            // Recheck the CanExecute property of the save command.
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public override async Task LoadAsync(int id)
        {
            Id = id;

            foreach (var wrapper in ProgrammingLanguages)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            ProgrammingLanguages.Clear();

            var languages = await programmingLanguageRepository.GetAllAsync();

            foreach (var model in languages)
            {
                var wrapper = new ProgrammingLanguageWrapper(model, languageValidator);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                ProgrammingLanguages.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = programmingLanguageRepository.HasChanges();
            }
            if (e.PropertyName == nameof(ProgrammingLanguageWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Will NOT be implemented (will not allow deletion of a whole collection 
        /// of programming languages), so leave as is.
        /// </summary>
        protected override void OnDeleteExecuteAsync()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && ProgrammingLanguages.All(plw => !plw.HasErrors);
        }

        protected override async void OnSaveExecuteAsync()
        {
            await SaveWithOptimisticConcurrencyAsync(programmingLanguageRepository.SaveAsync,
                () =>
                //try
                {
                    //await programmingLanguageRepository.SaveAsync();
                    HasChanges = programmingLanguageRepository.HasChanges();
                    base.RaiseCollectionSavedEvent();
                });
            //catch (Exception ex)
            //{

            //    while (ex.InnerException != null)
            //    {
            //        ex = ex.InnerException;
            //    }
            //    messageDialogService.ShowInfoDialog("Error while saving the entities, " +
            //        "the data will be reloaded. Details: " + ex.Message);
            //    await LoadAsync(Id);
            //}
        }
    }
}
