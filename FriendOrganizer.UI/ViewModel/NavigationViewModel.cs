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
        private readonly ILookupDataService<Friend> friendLookupService;
        private readonly IEventAggregator eventAggregator;

        public NavigationViewModel(ILookupDataService<Friend> friendLookupService,
            IEventAggregator eventAggregator)
        {
            this.friendLookupService = friendLookupService;
            this.eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            eventAggregator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);
            eventAggregator.GetEvent<AfterFriendDeletedEvent>().Subscribe(AfterFriendDeleted);
        }

        private void AfterFriendDeleted(int friendId)
        {
            NavigationItemViewModel lookupItem = Friends.SingleOrDefault(l => l.Id == friendId);
            if (lookupItem != null)
            {
                Friends.Remove(lookupItem);
            }
        }

        /// <summary>
        /// When the <see cref="AfterFriendSavedEvent"/> is raised, gets the Id of the saved <see cref="Friend"/>, and its new
        /// DisplayMember value passed on by the event and updates the appropriate <see cref="NavigationItemViewModel"/>
        /// in the <see cref="Friends"/> collection.
        /// </summary>
        /// <param name="args"></param>
        private void AfterFriendSaved(AfterFriendSavedEventsArgs args)
        {
            NavigationItemViewModel lookupItem = Friends.SingleOrDefault(l => l.Id == args.Id);
            // If no lookupItem was found...
            if (lookupItem == null)
            {
                // Add a new NavigationItemViewModel to the Friends collection of view models
                Friends.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, eventAggregator));
            }
            // Otherwise, simply update the DisplayMember.
            else
            {
                lookupItem.DisplayMember = args.DisplayMember;
            }        
        }

        /// <summary>
        /// Gets the observable collection of <see cref="NavigationItemViewModel"/>
        /// </summary>
        public ObservableCollection<NavigationItemViewModel> Friends { get; }

        /// <summary>
        /// Loads the <see cref="Friends"/> collection from the <see cref="friendLookupService"/>
        /// </summary>
        /// <returns>An observable collection of <see cref="NavigationItemViewModel"/> </returns>
        public async Task LoadAsync()
        {
            var lookup = await friendLookupService.GetLookupAsync();
            Friends.Clear();
            foreach (var item in lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, eventAggregator));
            }
        }
    }
}
