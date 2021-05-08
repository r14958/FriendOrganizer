using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly IFriendLookupDataServiceAsync friendLookupService;
        private readonly IEventAggregator eventAggregator;
        private NavigationItemViewModel selectedFriend;

        public NavigationViewModel(IFriendLookupDataServiceAsync friendLookupService,
            IEventAggregator eventAggregator)
        {
            this.friendLookupService = friendLookupService;
            this.eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            eventAggregator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);
        }

        private void AfterFriendSaved(AfterFriendSavedEventsArgs args)
        {
            var lookupItem = Friends.Single(l => l.Id == args.Id);
            lookupItem.DisplayMember = args.DisplayMember;
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; }


        public NavigationItemViewModel SelectedFriend
        {
            get { return selectedFriend; }
            set 
            {
                if (value != selectedFriend)
                {
                    selectedFriend = value;
                    OnPropertyChanged();
                    if (selectedFriend != null)
                    {
                        eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                            .Publish(selectedFriend.Id);
                    }
                }

            }
        }


        public async Task LoadAsync()
        {
            var lookup = await friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember));
            }
        }
    }
}
