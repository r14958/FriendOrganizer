using FluentValidation;
using FriendOrganizer.DataAccess.Services.Repositories;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendPhoneNumberListViewModel : ViewModelBase, IViewModel<FriendPhoneNumber>
    {
        private FriendPhoneNumberWrapper selectedPhoneNumber;
        private readonly IPhoneNumbersRepository phoneNumbersRepository;
        private readonly IEventAggregator eventAggregator;
        private readonly IValidator<FriendPhoneNumber> validator;

        public FriendPhoneNumberListViewModel(
            IPhoneNumbersRepository phoneNumbersRepository,
            IValidator<FriendPhoneNumber> validator,
            IEventAggregator eventAggregator)
        {
            this.phoneNumbersRepository = phoneNumbersRepository;
            this.validator = validator;
            this.eventAggregator = eventAggregator;
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumber);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumber, CanRemovePhoneNumber);

            this.eventAggregator.GetEvent<AfterFriendSavedEvent>().Subscribe(OnFriendSavedAsync);

            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();

        }

        private async void OnFriendSavedAsync(AfterFriendSavedEventsArgs obj)
        {
            await phoneNumbersRepository.SaveAsync();

            HasChanges = phoneNumbersRepository.HasChanges();
        }

        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get { return selectedPhoneNumber; }
            set
            {
                selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }


        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        public bool HasChanges { get; set; }

        public async Task LoadAsync(int? id)
        {
            ClearPhoneNumbers();

            if (id.HasValue)
            {
                IEnumerable<FriendPhoneNumber> numbers = await phoneNumbersRepository.GetAllPhoneNumbersForFriend(id);
                foreach (var number in numbers)
                {
                    var wrapper = new FriendPhoneNumberWrapper(number, validator);

                    PhoneNumbers.Add(wrapper);
                    var test = wrapper.HasErrors;
                    wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
                }
            }

        }

        private void ClearPhoneNumbers()
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }

            PhoneNumbers.Clear();
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = phoneNumbersRepository.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                eventAggregator.GetEvent<PhoneNumberChangedEvent>().Publish(((FriendPhoneNumberWrapper)sender).HasErrors);
            }
        }

        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }

        private FriendPhoneNumber CreateNewPhoneNumbers()
        {
            throw new NotImplementedException();
        }
        private bool CanRemovePhoneNumber()
        {
            return SelectedPhoneNumber != null;
        }

        private void OnRemovePhoneNumber()
        {
            throw new NotImplementedException();
        }

        private void OnAddPhoneNumber()
        {
            throw new NotImplementedException();
        }
    }
}
