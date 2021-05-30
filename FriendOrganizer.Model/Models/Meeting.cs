using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FriendOrganizer.Domain.Models
{
    public class Meeting : EntityBase
    {

        public Meeting()
        {
            Friends = new Collection<Friend>();
        }
        public string Title { get; set; }

        public DateTimeOffset DateFrom { get; set; }

        public DateTimeOffset DateTo { get; set; }

        public ICollection<Friend> Friends { get; set; }
    }
}
