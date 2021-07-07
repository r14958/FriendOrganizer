using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Commands;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        private readonly IProgrammingLanguageRepository programmingLanguageRepository;
        private ProgrammingLanguageWrapper selectedProgrammingLanguage;

        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IProgrammingLanguageRepository programmingLanguageRepository,
            IValidator<ProgrammingLanguage> languageValidator,
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Programming Languages";
            this.programmingLanguageRepository = programmingLanguageRepository;
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
            var wrapper = new ProgrammingLanguageWrapper(new ProgrammingLanguage());
            // Add a property changed event handler to the wrapped entity.
            wrapper.PropertyChanged += ProgrammingLanguage_PropertyChanged;
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
            SelectedProgrammingLanguage.PropertyChanged -= ProgrammingLanguage_PropertyChanged;
            
            // Instruct the repository to remove the entity from it DbContext.
            programmingLanguageRepository.Remove(SelectedProgrammingLanguage.Model);
            
            // Remove the wrapped entity from the public collection.
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage);
            
            // Null out the wrapped entity.
            SelectedProgrammingLanguage = null;
            
            // Recheck the CanExecute property of the save command.
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }


        public override async Task LoadAsync(int id)
        {
            Id = id;

            foreach (var programmingLanguage in ProgrammingLanguages)
            {
                programmingLanguage.PropertyChanged -= ProgrammingLanguage_PropertyChanged;
            }

            ProgrammingLanguages.Clear();

            var languages = await programmingLanguageRepository.GetAllAsync();

            foreach (var language in languages)
            {
                var programmingLanguage = new ProgrammingLanguageWrapper(language);
                programmingLanguage.PropertyChanged += ProgrammingLanguage_PropertyChanged;

                ProgrammingLanguages.Add(programmingLanguage);
            }
        }

        private void ProgrammingLanguage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var programmingLanguage = (ProgrammingLanguageWrapper)sender;


            if(e.PropertyName == nameof(programmingLanguage.IsChanged))
            {
                HasChanges = programmingLanguage.IsChanged;
                base.InvalidateControls();
            }

            if (e.PropertyName == nameof(programmingLanguage.HasErrors))
            {
                base.InvalidateControls();
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
            return GetHasChanges() && ProgrammingLanguages.All(pl => !pl.HasErrors);
        }

        protected override async void OnSaveExecuteAsync()
        {
            await SaveWithOptimisticConcurrencyAsync(programmingLanguageRepository.SaveAsync,
                () =>
                {
                    base.RaiseCollectionSavedEvent();
                });
            foreach (var programmingLanguage in ProgrammingLanguages)
            {
                programmingLanguage.AcceptChanges();
            }
            HasChanges = GetHasChanges();
        }

        protected override void OnResetExecuteAsync()
        {
            foreach (var programmingLanguage in ProgrammingLanguages)
            {
                programmingLanguage.RejectChanges();
            }
            HasChanges = GetHasChanges();
            InvalidateControls();
        }

        protected override bool OnResetCanExecute()
        {
            return GetHasChanges();
        }

        private bool GetHasChanges()
        {
            return ProgrammingLanguages.Any(pl => pl.IsChanged);
        }
    }
}
