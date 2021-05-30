using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        protected readonly IEventAggregator eventAggregator;
        private bool hasChanges;

        public DetailViewModelBase(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            SaveCommand = new DelegateCommand(OnSaveExecuteAsync, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecuteAsync, OnDeleteCanExecute);

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

        public abstract Task LoadAsync(int? id);

        public ICommand SaveCommand { get; private set; }
        
        public ICommand DeleteCommand { get; private set; }

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

        protected virtual void RaiseDetailDeletedEvent(int entityId)
        {
            eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                    .Publish(new AfterDetailDeletedEventArgs
                    {
                        Id = entityId,
                        ViewModelName = GetType().Name
                    });
        }
        
    }
}
