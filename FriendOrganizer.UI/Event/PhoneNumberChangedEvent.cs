using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Event
{
    /// <summary>
    /// Event that signals when a phone number is changed
    /// </summary>
    /// <remarks>
    /// Payload is a boolean whether the phone number has an error.
    /// </remarks>
    public class PhoneNumberChangedEvent : PubSubEvent<bool>
    {
    }
}
