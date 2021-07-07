using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    public class ChangeTrackingCollection<T> : ObservableCollection<T>, IValidatableTrackingObject
        where T : class, IValidatableTrackingObject
    {
        private IList<T> originalCollection;

        private readonly ObservableCollection<T> addedItems;
        private readonly ObservableCollection<T> removedItems;
        private readonly ObservableCollection<T> modifiedItems;

        public ChangeTrackingCollection(IEnumerable<T> items) : base(items)
        {
            originalCollection = this.ToList();

            AttachItemPropertyChangedHandler(originalCollection);

            addedItems = new ObservableCollection<T>();
            removedItems = new ObservableCollection<T>();
            modifiedItems = new ObservableCollection<T>();

            AddedItems = new ReadOnlyObservableCollection<T>(addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(modifiedItems);
        }

        public ReadOnlyObservableCollection<T> AddedItems { get; private set; }
        public ReadOnlyObservableCollection<T> RemovedItems { get; private set; }
        public ReadOnlyObservableCollection<T> ModifiedItems { get; private set; }

        public bool IsChanged => AddedItems.Count > 0 || ModifiedItems.Count > 0 || RemovedItems.Count > 0;

        public bool IsValid => this.All(t => t.IsValid);

        private void AttachItemPropertyChangedHandler(IList<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }
        
        private void DetachItemPropertyChangedHandler(IList<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the IsValid property changed, raise its OnPropertyChanged event handler.
            if (e.PropertyName == nameof(IsValid))
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
            }
            
            // For all other property changes, track them in the change tracking dictionaries.
            else
            {
                // Cast the sender to the appropriate type.
                var item = (T)sender;

                // If the item is already in the added Items collection, do nothing...
                // Only care if item is added, not if it is changed after being added
                // but before it is saved.
                if (addedItems.Contains(item)) return;

                // Otherwise, if the item is changed from its original value...
                if (item.IsChanged)
                {
                    //...and its not in the modified items collection...
                    if (!modifiedItems.Contains(item))
                    {
                        //...add it to the modified collection.
                        modifiedItems.Add(item);
                    }
                }
                // Otherwise, if is NOT changed from its original value...
                else
                {
                    // ...but it is in the modified items collection...
                    if (modifiedItems.Contains(item))
                    {
                        // ...remove it from the modified collection.
                        modifiedItems.Remove(item);
                    }
                }

                // Fire the changed event for the IsChanged property of ChangeTrackingCollection.
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged))); 
            }
        }

        /// <summary>
        /// Accepts all the changes by clearing out the subcollections, and resetting the orginal collection to match the current collection.
        /// </summary>
        public void AcceptChanges()
        {
            addedItems.Clear();
            removedItems.Clear();
            modifiedItems.Clear();

            // Accept changes for all the items in the collection, which will clear out their tracking dictionaries, and reset their original
            // values to their current values.
            foreach (var item in this)
            {
                item.AcceptChanges();
            }

            // Update the originalCollection to match the current tracking collection
            originalCollection = this.ToList();

            // Let the UI (and other listeners) know the IsChanged property of change tracking collection has been updated.
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        /// <summary>
        /// Reverses all the changes to the ChangeTrackingCollection so that it once again matches the originalCollection.
        /// </summary>
        public void RejectChanges()
        {
            // Remove all addedItems from the collection.
            foreach (var addedItem in addedItems.ToList())
            {
                Remove(addedItem);
            }

            // Add back in any items that where removed from the collection.
            foreach (var removedItem in removedItems.ToList())
            {
                // In case the item was modified and then removed, need to reject the changes
                // first, otherwise, it will get "stuck" in the modifiedItems collection.
                removedItem.RejectChanges();
                // Now can add it back to the tracked collection
                Add(removedItem);
            }

            // Reject all changes to any modified items, restoring them to their original state.
            foreach (var modifiedItem in modifiedItems.ToList())
            {
                modifiedItem.RejectChanges();
            }

            // Let the UI (and other listeners) know the IsChanged property of change tracking collection has been updated.
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        /// <summary>
        /// Override of the base ObservableCollection class, this is fired everytime the collection changes.
        /// This compares the current collection to the original collection and determines what items have been
        /// added and removed.  Any of the remainders that have their IsChanged = true are added to the modified collection.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // The collection of added items are all those items in the current tracked collection which
            // do not exist in the original collection.
            var added = this.Where(current => originalCollection.All(orig => orig != current)).ToList();

            // The collection of removed items are those that where in the original collection but
            // do not exist in the current collection.
            var removed = originalCollection.Where(orig => this.All(current => current != orig)).ToList();

            // Modified items in the current collection excluding the added and removed items and
            // where their IsChanged property is true.
            var modified = this.Except(added).Except(removed).Where(item => item.IsChanged).ToList();

            // Add or remove property changed handlers to the added and removed items in the tracking collection.
            AttachItemPropertyChangedHandler(added);
            DetachItemPropertyChangedHandler(removed);

            // Reset the three tracking sub-collections
            UpdateObservableCollection(addedItems, added);
            UpdateObservableCollection(removedItems, removed);
            UpdateObservableCollection(modifiedItems, modified);

            base.OnCollectionChanged(e);

            // Let the UI (and other listeners) know the IsChanged property of change tracking collection has been updated.
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            //TODO: Should this be uncommented? Unit tests pass without it!
            //OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
        }
        /// <summary>
        /// Replaces all entries in an existing observable collection with a new list of members.
        /// </summary>
        /// <param name="observableCollection">The <see cref="ObservableCollection{T}"/> that is being refilled.</param>
        /// <param name="items">The list of items that are refilling the collection.</param>
        private static void UpdateObservableCollection(ObservableCollection<T> observableCollection, List<T> items)
        {
            observableCollection.Clear();
            foreach (var item in items)
            {
                observableCollection.Add(item);
            }
        }
    }
}
