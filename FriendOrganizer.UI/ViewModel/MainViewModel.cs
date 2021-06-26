using Autofac.Features.Indexed;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.ViewModel.Factory;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static FriendOrganizer.UI.Utilities;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : NotifyPropChangedBase
    {
        private readonly CreateDetailViewModel createDetailViewModel;
        private readonly IEventAggregator eventAggregator;
        private readonly IMessageDialogService messageDialogService;
        private IDetailViewModel selectedDetailViewModel;
        private int nextNewItemId = 0;

        public MainViewModel(
            INavigationViewModel navigationViewModel,
            CreateDetailViewModel createDetailViewModel,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
        {
            this.createDetailViewModel = createDetailViewModel;
            this.eventAggregator = eventAggregator;
            this.messageDialogService = messageDialogService;

            this.eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);
            this.eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(OnAfterDetailDeleted);
            this.eventAggregator.GetEvent<AfterDetailCloseEvent>()
                .Subscribe(OnAfterDetailClosed);
            this.eventAggregator.GetEvent<AfterAllDetailCloseEvent>()
                .Subscribe(OnAfterAllDetailClosed);
            this.eventAggregator.GetEvent<AfterAllDetailSaveEvent>()
                .Subscribe(OnAfterAllDetailSaved);

            NavigationViewModel = navigationViewModel;

            DetailViewModels = new ObservableCollection<IDetailViewModel>();

            // Note that we are using a DelegateCommand<T>, where T is the type of the command parameter.
            // In this case, our parameter is of type Type, and is relaying the type of ViewModel to load.
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);

            OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailViewExecute);

        }


        /// <summary>
        /// Gets the <see cref="ViewModel.NavigationViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Set in the class's constructor, so no setter is needed.</remarks>
        public INavigationViewModel NavigationViewModel { get; }


        /// <summary>
        /// Gets the <see cref="ViewModel.IDetailViewModel"/> property for the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <remarks>Private setter, since this is only set during the <see cref="OnOpenDetailView(OpenDetailViewEventArgs)"/> event.</remarks>
        public IDetailViewModel SelectedDetailViewModel
        {
            get { return selectedDetailViewModel; }
            set 
            { 
                selectedDetailViewModel = value;
                OnPropertyChanged();

            }
        }

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        /// <summary>
        /// L
        /// </summary>
        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewDetailCommand { get; }

        public ICommand OpenSingleDetailViewCommand { get; }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            // Check to see if the collection of DetailViewModels already contains the requested DVM.
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == args.Id
                && vm.GetType().Name == args.ViewModelName);

            // If not, create one, populate it, and add it to the collection.
            // Note: since any new DVM will have Id = 0, this means only one new DVM
            // can be created at any given time.
            if (detailViewModel == null)
            {
                detailViewModel = createDetailViewModel(args.ViewModelName);

                try
                {
                    await detailViewModel.LoadAsync(args.Id);
                }
                catch
                {
                    await messageDialogService.ShowInfoDialogAsync("Could not load the entity, it may have been deleted " +
                        "in the meantime by another user. The navigation will be refreshed for you.");

                    await NavigationViewModel.LoadAsync();
                    return;
                }                
                DetailViewModels.Add(detailViewModel);
            }
            
            // Set the selected DetailViewModel to either the found or new DVM
            SelectedDetailViewModel = detailViewModel;
        }


        /// <summary>
        /// Creates a new <see cref="SelectedDetailViewModel"/> and populates it. 
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>Will check for changes to the existing <see cref="SelectedDetailViewModel"/> and verify with the user before 
        /// navigating away without saving.</remarks>
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            // By using the int decrement operator for the Id property, every new
            // DetailViewModel gets its own unique Id, which is always less than zero.
            // This allows multiple new DVMs to be created and referenced simultaneously
            // before they are saved to the DB and given proper Id values.
            OnOpenDetailView(new OpenDetailViewEventArgs 
                {
                    Id = nextNewItemId--, 
                    ViewModelName = viewModelType.Name 
                });
        }

        private void OnOpenSingleDetailViewExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs
            {
                Id = -1,
                ViewModelName = viewModelType.Name
            });
        }

        /// <summary>
        /// Based on the information passed on by the event, find the closed
        /// DetailViewModel in the collection and remove it.
        /// </summary>
        /// <param name="args"></param>
        private void OnAfterDetailClosed(AfterDetailCloseEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        /// <summary>
        /// Based on the information passed on by the event, find the deleted
        /// DetailViewModel in the collection and remove it.
        /// </summary>
        /// <param name="args"></param>
        private void OnAfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void RemoveDetailViewModel(int id, string viewModelName)
        {
            var detailviewModel = DetailViewModels
                            .SingleOrDefault(vm => vm.Id == id
                            && vm.GetType().Name == viewModelName);

            if (detailviewModel != null)
            {
                DetailViewModels.Remove(detailviewModel);
            }
        }

        private void OnAfterAllDetailClosed()
        {

            // Note that we are iterating threw an anonymous list that is a copy of the 
            // DetailViewModels collection.  This allow us to modify the collection in the loop.
            foreach (DetailViewModelBase detailViewModel in DetailViewModels.ToList())
            {
                // Tell the detailViewModel to close itself.
                detailViewModel.CloseDetailViewCommand.Execute(null);
            }
        }

        private void OnAfterAllDetailSaved()
        {

            // Note that we are iterating threw an anonymous list that is a copy of the 
            // DetailViewModels collection.  This allow us to modify the collection in the loop.
            foreach (DetailViewModelBase detailViewModel in DetailViewModels.ToList())
            {
                if (detailViewModel.HasChanges)
                {
                    // Tell the detailViewModel to close itself.
                    detailViewModel.SaveCommand.Execute(null);
                }
            }
        }
    }
}
