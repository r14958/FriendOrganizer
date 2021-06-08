using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        protected readonly IEventAggregator eventAggregator;
        protected readonly IMessageDialogService messageDialogService;
        private bool hasChanges;
        private int id;
        private string title;

        public DetailViewModelBase(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            this.eventAggregator = eventAggregator;
            this.messageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecuteAsync, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecuteAsync, OnDeleteCanExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailExecute);

        }

        
        public int Id
        {
            get { return id; }
            protected set { id = value; }
        }

        public string Title
        {
            get { return title; }
            set 
            {
                if (value != title)
                {
                    title = value;
                    OnPropertyChanged();
                }

            }
        }

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

        public abstract Task LoadAsync(int id);

        public ICommand SaveCommand { get; private set; }
        
        public ICommand DeleteCommand { get; private set; }

        public ICommand CloseDetailViewCommand { get; private set; }

        protected virtual bool OnDeleteCanExecute()
        {
            return true;
        }

        protected abstract void OnDeleteExecuteAsync();
        
        protected abstract bool OnSaveCanExecute();
        
        protected abstract void OnSaveExecuteAsync();

        protected virtual void RaiseDetailSavedEvent(int entityId, string displayMember)
        {
            eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Publish(
                new AfterDetailSavedEventsArgs
                {
                    Id = entityId,
                    DisplayMember = displayMember,
                    ViewModelName = GetType().Name
                });
        }

        /// <summary>
        /// Raises the <see cref="AfterCollectionSavedEvent"/>.
        /// </summary>
        /// <remarks>Passes on the name of the ViewModel which saved the collection.</remarks>
        protected virtual void RaiseCollectionSavedEvent()
        {
            eventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Publish(
                new AfterCollectionSavedEventArgs
                {
                    ViewModelName = GetType().Name
                });
        }
        
        protected virtual void RaiseDetailDeletedEvent(int entityId)
        {
            eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                    .Publish(new AfterDetailDeletedEventArgs
                    {
                        Id = entityId,
                        ViewModelName = GetType().Name
                    });
        }

        protected virtual void OnCloseDetailExecute()
        {
            if (HasChanges)
            {
                var response = messageDialogService.ShowOKCancelDialog("You have made changes.  Close this item without saving?", "Question");
                if (response == MessageDialogResult.Cancel) return; 
            }

            eventAggregator.GetEvent<AfterDetailCloseEvent>()
                    .Publish(new AfterDetailCloseEventArgs
                    {
                        Id = this.Id,
                        ViewModelName = GetType().Name
                    });
        }

        protected async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc, Action afterSaveAction)
        {
            try
            {
                // Use the data repository to save the friend to the DB.
                await saveFunc();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    messageDialogService.ShowInfoDialog("The entity has been deleted by another user and can no longer be edited. " +
                        "Your navigation will now be updated.");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var result = messageDialogService.ShowOKCancelDialog("The entity has been changed in the meantime by " +
                    "someone else.  Click OK to save your changes anyway, click Cancel to reload the entity " +
                    "from the database.", "Question");

                if (result == MessageDialogResult.OK)
                {
                    // Update the original values with database-values
                    var entry = ex.Entries.Single();

                    // OriginalValues are the entity's values before the concurrency -- before two users tried to update the
                    // DB at nearly the same time.  The rowVersion of this entry is N.
                    //
                    // entry.GetDatabaseValues are the entity values from the save event during the concurrency event.  It is the
                    // first successful save during the concurrency.  The rowVersion of this entry is N + 1.

                    // CurrentValues are the values that the second (current) user entered -- the ones that have not been accepted
                    // yet.  The rowVersion of this entry is N.  This is how EF Core recognizes the concurrency: the original row version
                    // is behind the last saved, but the same as the current (latest) values.

                    // This line copies the DB (last saved) values into the OriginalValues.  So now the Original rowVersion is N + 1, ahead
                    // of the Current, so EF Core no longer sees a conflict.
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());

                    // The DBContext accepts the new values and saves them to the DB.
                    await saveFunc();

                    // After the above save, Original, DB, and CurrentValues are all the same -- the ones saved by the last User and the rowVersion
                    // is N+2.  These values and rowVersion will become the "Original" values when the next concurrency event occurs.


                }
                else
                {
                    //Reload entity from the database (from the last successful save)
                    await ex.Entries.Single().ReloadAsync();

                    // Resync everything in FriendDetailViewModel with these "recovered" values.
                    await LoadAsync(Id);
                }
            }

            afterSaveAction();
        }
    }
}
