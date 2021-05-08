using FriendOrganizer.Domain;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel(INavigationViewModel navigationViewModel,
            IFriendDetailViewModel friendDetailViewModel) 
        {
            NavigationViewModel = navigationViewModel;
            FriendDetailViewModel = friendDetailViewModel;

        }

        /// <summary>
        /// Gets the <see cref="ViewModel.NavigationViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Set in the class's constructor, so no setter is needed.</remarks>
        public INavigationViewModel NavigationViewModel { get; }

        /// <summary>
        /// Gets the <see cref="ViewModel.FriendDetailViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Set in the class's constructor, so no setter is needed.</remarks>
        public IFriendDetailViewModel FriendDetailViewModel { get; }
        
        /// <summary>
        /// Asynchronous Load method of the <see cref="ViewModel.NavigationViewModel"/>.
        /// </summary>
        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }
    }
}
