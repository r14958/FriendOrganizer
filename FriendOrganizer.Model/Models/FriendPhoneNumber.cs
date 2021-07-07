using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Models
{
    [DebuggerDisplay("Friend {Friend.FullName}, Id: {Id}, Ph#: {Number}, Comment: {Comment}")]
    public class FriendPhoneNumber : EntityBase
    {
        public string Number { get; set; }

        public string Comment { get; set; }

        public int FriendId { get; set; }

        public Friend Friend { get; set; }
    }
}
