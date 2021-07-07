using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FriendOrganizer.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Title: {Title}")]
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
