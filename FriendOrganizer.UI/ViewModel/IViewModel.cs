using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public interface IViewModel : INotifyPropertyChanged
    {
        public virtual void Dispose() { }
    }
}
