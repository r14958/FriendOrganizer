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
        private readonly ILookupDataService<Meeting> meetingLookupService;
        private readonly IEventAggregator eventAggregator;

        public NavigationViewModel(ILookupDataService<Friend> friendLookupService,
            ILookupDataService<Meeting> meetingLookupService,
            IEventAggregator eventAggregator)
        {
            this.friendLookupService = friendLookupService;
            this.meetingLookupService = meetingLookupService;
            this.eventAggregator = eventAggregator;

            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();

            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSavedExecute);
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeletedExecute);
        }
        
        public ObservableCollection<NavigationItemViewModel> Friends { get; }

        public ObservableCollection<NavigationItemViewModel> Meetings { get; }

        /// <summary>
        /// Loads the <see cref="Friends"/> collection from the <see cref="friendLookupService"/>
        /// </summary>
        /// <returns>An observable collection of <see cref="NavigationItemViewModel"/> </returns>
        public async Task LoadAsync()
        {
            var friends = await friendLookupService.GetLookupAsync();
            Friends.Clear();
            foreach (var friend in friends)
            {
                Friends.Add(new NavigationItemViewModel(friend.Id, friend.DisplayMember, eventAggregator, nameof(FriendDetailViewModel)));
            }

            var meetings = await meetingLookupService.GetLookupAsync();
            Meetings.Clear();
            foreach (var meeting in meetings)
            {
                Meetings.Add(new NavigationItemViewModel(meeting.Id, meeting.DisplayMember, eventAggregator, nameof(MeetingDetailViewModel)));
            }
        }

        private void AfterDetailSavedExecute(AfterDetailSavedEventsArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailSavedEvent(Friends, args);       
                    break;

                case nameof(MeetingDetailViewModel):
                    AfterDetailSavedEvent(Meetings, args);
                    break;
            }
        }

        private void AfterDetailSavedEvent(ObservableCollection<NavigationItemViewModel> items, 
            AfterDetailSavedEventsArgs args)
        {
            NavigationItemViewModel item = items.SingleOrDefault(i => i.Id == args.Id);
            // If no lookupItem was found...
            if (item == null)
            {
                // Add a new NavigationItemViewModel to the Friends collection of view models
                items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, eventAggregator,
                    args.ViewModelName));
            }
            // Otherwise, simply update the DisplayMember.
            else
            {
                item.DisplayMember = args.DisplayMember;
            }
        }

        private void AfterDetailDeletedExecute(AfterDetailDeletedEventArgs args)
        {

            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailDeleted(Friends, args);
                    break;

                case nameof(MeetingDetailViewModel):
                    AfterDetailDeleted(Meetings, args);
                    break;
            }
        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, 
            AfterDetailDeletedEventArgs args)
        {
            NavigationItemViewModel item = items.SingleOrDefault(i => i.Id == args.Id);
            if (item != null)
            {
                items.Remove(item);
            }
        }
    }
}
