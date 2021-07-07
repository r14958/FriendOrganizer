using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FriendOrganizer.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Name: {FullName}")]
    public class Friend : EntityBase
    {
        private string firstName;
        private string lastName;

        public Friend()
        {
            PhoneNumbers = new List<FriendPhoneNumber>();
            Meetings = new Collection<Meeting>();
            Address = new();
        }

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

        public Address Address { get; set; }

        public string FullName => FirstName + ' ' + LastName;

        public int? FavoriteLanguageId { get; set; }

        public bool IsDeveloper { get; set; }

        public ProgrammingLanguage FavoriteLanguage { get; set; }

        public List<FriendPhoneNumber> PhoneNumbers { get; set; }

        public ICollection<Meeting> Meetings { get; set; }
    }
}
