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
        private readonly string detailViewModelName;
        private string displayMember;

        public NavigationItemViewModel(int id, string displayMember, IEventAggregator eventAggregator,
            string detailViewModelName)
        {
            this.Id = id;
            this.displayMember = displayMember;
            this.eventAggregator = eventAggregator;
            this.detailViewModelName = detailViewModelName;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
        }

       
        /// <summary>
        /// Gets the ID of the Item.  
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
        
        public ICommand OpenDetailViewCommand { get; }

        /// <summary>
        /// Fires the <see cref="OpenDetailViewEvent"/> publishing <see cref="Id"/> of the selected Friend.
        /// </summary>
        private void OnOpenDetailViewExecute()
        {
            eventAggregator.GetEvent<OpenDetailViewEvent>()
                            .Publish(
                new OpenDetailViewEventArgs
                {
                    Id = Id,
                    ViewModelName = detailViewModelName
                });
        }
    }

}
