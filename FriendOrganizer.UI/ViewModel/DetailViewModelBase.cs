﻿using FriendOrganizer.UI.Commands;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.Wrapper;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : NotifyPropChangedBase, IDetailViewModel
    {
        protected readonly IEventAggregator eventAggregator;
        protected readonly IMessageDialogService messageDialogService;
        private int id;
        private string title;
        private bool hasChanges;

        public DetailViewModelBase(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            this.eventAggregator = eventAggregator;
            this.messageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecuteAsync, OnSaveCanExecute);
            ResetCommand = new DelegateCommand(OnResetExecuteAsync, OnResetCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecuteAsync, OnDeleteCanExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailExecute);
            CloseAllCommand = new DelegateCommand(OnCloseAllDetailExecute);
            SaveAllCommand = new DelegateCommand(OnSaveAllDetailExecute, OnSaveAllDetailCanExecute);

        }

        protected virtual bool OnSaveAllDetailCanExecute()
        {
            return true;
        }

        private void OnSaveAllDetailExecute()
        {
            eventAggregator.GetEvent<AfterAllDetailSaveEvent>().Publish();
        }

        private void OnCloseAllDetailExecute()
        {
            eventAggregator.GetEvent<AfterAllDetailCloseEvent>().Publish();
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

        public virtual bool HasAnyChanges { get; }

        public bool HasChanges
        {
            get { return hasChanges; }
            set
            {
                if (hasChanges != value)
                {
                    hasChanges = value;
                    OnPropertyChanged();
                    InvalidateControls();
                }
            }
        }

        public abstract Task LoadAsync(int id);

        public ICommand CloseAllCommand { get; private set; }
        
        public ICommand SaveAllCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }
        
        public ICommand ResetCommand { get; private set; }
        
        public ICommand DeleteCommand { get; private set; }

        public ICommand CloseDetailViewCommand { get; private set; }

        protected static bool AllHaveAnyChanges { get; private set; }

        protected virtual bool OnDeleteCanExecute()
        {
            return true;
        }

        protected abstract void OnDeleteExecuteAsync();

        protected virtual bool OnResetCanExecute()
        { 
            return true; 
        }


        protected abstract void OnResetExecuteAsync();
        

        protected virtual bool OnSaveCanExecute()
        {
            return true;
        }
        
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

        protected async virtual void OnCloseDetailExecute()
        {
            if (!HasChanges)
            {
                SendCloseEvent();
            }
            else
            {    
                var response = await messageDialogService.ShowOKCancelDialogAsync($"You have made changes. Close {Title} without saving?", "Question");
                if (response == MessageDialogResult.OK)
                {
                    
                    SendCloseEvent();
                }
            }
        }

        private void SendCloseEvent()
        {
            OnResetExecuteAsync();
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
                    await messageDialogService.ShowInfoDialogAsync("The entity has been deleted by another user and can no longer be edited. " +
                        "Your navigation will now be updated.");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var result = await messageDialogService.ShowOKCancelDialogAsync("The entity has been changed in the meantime by " +
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

        protected virtual void InvalidateControls()
        {
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ResetCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ResetCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveAllCommand).RaiseCanExecuteChanged();
        }
    }
}
