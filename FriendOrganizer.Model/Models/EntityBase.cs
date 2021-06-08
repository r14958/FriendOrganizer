using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.Domain.Models

{
    /// <summary>
    /// Base class for all Entities
    /// </summary>
    public class EntityBase : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int Version { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;



        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Optional string.  If omitted, will invoke the <see cref="CallerMemberNameAttribute"/>
        /// to automatically use the correct property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
