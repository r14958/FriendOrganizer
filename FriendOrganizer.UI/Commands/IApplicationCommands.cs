using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Commands
{
    public interface IApplicationCommands
    {
        CompositeCommand SaveAllCommand { get; }
        CompositeCommand CloseAllCommand { get; }
    }
}
