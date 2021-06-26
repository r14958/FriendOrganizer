using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Commands
{
    public class ApplicationCommands : IApplicationCommands
    {
        public ApplicationCommands()
        {
            SaveAllCommand = new();
            CloseAllCommand = new();
        }
        
        public CompositeCommand SaveAllCommand { get; }
        public CompositeCommand CloseAllCommand { get; }
    }
}
