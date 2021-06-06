using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using Prism.Commands;
using Prism.Events;
using System;
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

    }
}
