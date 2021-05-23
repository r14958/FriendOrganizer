using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Models
{
    public class FriendPhoneNumber : EntityBase
    {
        public string Number { get; set; }

        public int FriendId { get; set; }

        public Friend Friend { get; set; }
    }
}
