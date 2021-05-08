using System;
using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Domain.Models
{
    public class Friend : EntityBase
    {
        private string firstName;
        private string lastName;

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
            }
        }
        public string LastName
        {
            get => lastName; 
            set
            {
                lastName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
            }
        }
        public string Email { get; set; }

        public string FullName => FirstName + ' ' + LastName;


    }
}
