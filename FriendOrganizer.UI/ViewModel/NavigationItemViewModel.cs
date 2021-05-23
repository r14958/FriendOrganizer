using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    /// <summary>
    /// Encapsulates the minimum information to describe a Friend navigation entity: its Id and a description (displayMember).
    /// </summary>
    public class NavigationItemViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        private string displayMember;

        public NavigationItemViewModel(int id, string displayMember, IEventAggregator eventAggregator)
        {
            this.Id = id;
            this.displayMember = displayMember;
            this.eventAggregator = eventAggregator;

            OpenDetailFriendViewCommand = new DelegateCommand(OnOpenDetailFriendView);
        }

       
        /// <summary>
        /// Gets the ID of the Friend.  
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets and sets the DisplayMember (description) of the navigation entity.
        /// </summary>
        public string DisplayMember
        {
            get
            {
                return displayMember;
            }

            set
            {
                if (displayMember != value)
                {
                    displayMember = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public ICommand OpenDetailFriendViewCommand { get; }

        /// <summary>
        /// Fires the <see cref="OpenFriendDetailViewEvent"/> publishing <see cref="Id"/> of the selected Friend.
        /// </summary>
        private void OnOpenDetailFriendView()
        {
            eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                            .Publish(Id);
        }
    }

}
