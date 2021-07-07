using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Models
{
    [DebuggerDisplay("Name: {Friend.FullName}, Address: {StreetNumber} {Street}, {City}")]
    public class Address : EntityBase
    {
        public string City { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public int FriendId { get; set; }

        public Friend Friend { get; set; }
    }
}
