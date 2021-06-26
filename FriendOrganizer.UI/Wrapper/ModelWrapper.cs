using FluentValidation;
using FluentValidation.Results;
using DataAnnotations = System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Generic data Model wrapper
    /// </summary>
    /// <typeparam name="T">The type of the model being wrapped.</typeparam>
    public abstract class ModelWrapper<T> : NotifyDataErrorInfoBase, IRevertibleChangeTracking
    {
        protected readonly IValidator<T> validator;
        // Dictionary to store the original values of any simple, changed property in
        // the model that is being wrapped by this model wrapper.
        private readonly Dictionary<string, object> originalValues;
        // List of all the complex properties and collections contained in the model
        // that is be wrapped by this model wrapper.
        private readonly List<IRevertibleChangeTracking> trackingObjects;

        /// <summary>
        /// Constructor for this model wrapper.
        /// </summary>
        /// <param name="model">The model being wrapped.</param>
        /// <param name="validator">Optional: FluentValidation validator for this model.  If omitted, none
        /// will be applied.</param>
        /// <exception cref="ArgumentNullException"/>
        public ModelWrapper(T model, IValidator<T> validator=null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(Model), "The model being wrapped cannot be null.");
            }
            Model = model;
            this.validator = validator;

            originalValues = new();
            trackingObjects = new();
        }
        
        /// <summary>
        /// Gets the instance of class T being wrapped
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Returns whether any of the wrapper property values have changed, by checking
        /// whether the change tracking dictionary has any entries or any of the tracking-
        /// objects have changed.
        /// </summary>
        public virtual bool IsChanged => originalValues.Count > 0 || trackingObjects.Any(t => t.IsChanged);

        /// <summary>
        /// Sets the state of the model wrapper assuming all changes have been accepted.
        /// </summary>
        public virtual void AcceptChanges()
        {
            // Once all the changes have been accepted, clear the simple property change tracking dictionary.
            originalValues.Clear();
            DeltaChanges = 0;

            // Do the same for each trackingObject.
            foreach (var trackingObject in trackingObjects)
            {
                trackingObject.AcceptChanges();
            }

            // WPF trick: running OnPropertyChanged with any empty or null propertyName
            // triggers all bound UIElements to refresh their values from the ViewModel. 
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Sets the state of the model wrapper assuming all changes have been rejected.
        /// </summary>
        public virtual void RejectChanges()
        {
            // Cycle through every object in the change tracking dictionary.
            foreach (var originalValueEntry in originalValues)
            {
                // Since the dictionary keys are the property names, use Reflection to set
                // each property back to its original value.
                typeof(T).GetProperty(originalValueEntry.Key).SetValue(Model, originalValueEntry.Value);

                // Remove any tracked errors associated with this property name.
                ClearErrors(originalValueEntry.Key);

                DeltaChanges--;
            }

            // Clear out the change tracking dictionary.
            originalValues.Clear();

            

            // Do the same for each tracking object .
            foreach (var wrapper in trackingObjects)
            {
                wrapper.RejectChanges();
            }
            // Fire all OnPropertyChanged events in one shot.
            OnPropertyChanged(string.Empty);
        }

        public virtual void IgnoreChange(string propertyName)
        {
            if (originalValues.ContainsKey(propertyName))
            {
                originalValues.Remove(propertyName);
            }
        }

        /// <summary>
        /// Returns the value of a property of this model wrapper.
        /// </summary>
        /// <typeparam name="Tvalue">The type of property value being returned.</typeparam>
        /// <param name="propertyName">Optional: The name of the property whose value is being returned.
        /// If omitted, the name of the calling method or property will be used.</param>
        /// <returns>The value of the property.</returns>
        protected virtual Tvalue GetValue<Tvalue>([CallerMemberName] string propertyName = null)
        {
            return (Tvalue)typeof(T).GetProperty(propertyName).GetValue(Model);
        }

        protected virtual Tvalue GetOriginalValue<Tvalue>(string propertyName)
        {
            // If an original value is stored...
            if (originalValues.ContainsKey(propertyName))
            {
                // ...return it.
                return (Tvalue)originalValues[propertyName]; 
            }
            // Otherwise, 
            else
            {
                // ...the value has never changed, so just return the current value.
                return GetValue<Tvalue>(propertyName);
            }
        }

        /// <summary>
        /// Returns whether a property in this model wrapper has changed from its original value.
        /// </summary>
        /// <param name="propertyName">The name of the property being evaluated for changes.</param>
        /// <returns>True if the value has changed from its original; otherwise, False.</returns>
        protected virtual bool GetIsChanged(string propertyName)
        {
            // Only values that have changed are stored.
            return originalValues.ContainsKey(propertyName);
        }

        /// <summary>
        /// Assuming it has changed, sets the new value for a property in this model wrapper.
        /// </summary>
        /// <typeparam name="Tvalue">The type of the property value being set.</typeparam>
        /// <param name="newValue">The new value of property.</param>
        /// <param name="propertyName">Optional: The name of the property being updated. If omitted, the name
        /// of the calling method or property will be used.</param>
        protected virtual void SetValue<Tvalue>(Tvalue newValue, 
            [CallerMemberName] string propertyName = null)
        {
            var currentValue = GetValue<Tvalue>(propertyName);

            // If the property value did not change, do nothing...
            if (Equals(currentValue, newValue))
            {
                return;
            }

            //Otherwise, add it or remove it from our originalValues dictionary.
            UpdateOriginalValue(currentValue, newValue, propertyName);

            // Using Reflection, update the property of Model with the new value.
            typeof(T).GetProperty(propertyName).SetValue(Model, newValue);

            // Raise the property changed event from the base class
            OnPropertyChanged(propertyName);

            // If one exists, also raise one for the "IsChanged" flag for the same property.
            OnPropertyChanged(propertyName + "IsChanged");

            // Clear any existing validation errors for the property from the errors dictionary in the base class.
            ClearErrors(propertyName);
            ValidateErrors(newValue, propertyName);
        }

        private void ValidateErrors<Tvalue>(Tvalue newValue, string propertyName)
        {
            // If a validator was provided check for FluentValidation errors.
            if (validator != null) ValidateFluentErrors();

            // Then validate based on data annotations.
            ValidateDataAnnotations(newValue, propertyName);
        }

        /// <summary>
        /// Keeps the model collection in sync with its wrapper collection.
        /// </summary>
        /// <typeparam name="TWrapper">The type of wrapper collection.</typeparam>
        /// <typeparam name="TModel">The type of model collection.</typeparam>
        /// <param name="wrapperCollection">The wrapper collection being synchronized.</param>
        /// <param name="modelCollection">The model collection being synchronized.</param>
        protected void RegisterCollection<TWrapper, TModel>(
            ChangeTrackingCollection<TWrapper> wrapperCollection, 
            List<TModel> modelCollection) where TWrapper : ModelWrapper<TModel>
        {
            // Every time the wrapper collection is changed...
            wrapperCollection.CollectionChanged += (s, e) =>
            {
                // Clear out the model collection.
                modelCollection.Clear();
                // Add back in all the models in the wrapper collection.
                modelCollection.AddRange(wrapperCollection.Select(w => w.Model));
            };
            // Add property changed 
            RegisterTrackingObject(wrapperCollection);
        }

        /// <summary>
        /// Checks if a complex wrapper is already in the tracked complex wrapper List,
        /// and if not, adds it to the list.
        /// </summary>
        /// <typeparam name="TModel">The type of (complex) model being wrapped.</typeparam>
        /// <param name="wrapper">The complex model wrapper being registered.</param>
        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObject(wrapper);
        }

        private void RegisterTrackingObject<TTrackingObject>(TTrackingObject trackingObject)
            where TTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
        {
            if (!trackingObjects.Contains(trackingObject))
            {
                trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        /// <summary>
        /// If the IsChanged property of any of the tracked complex wrappers in this model wrapper changes, then also fire
        /// // the IsChanged property of this model wrapper.
        /// </summary>
        /// <param name="sender">The tracked complex wrapper that changed.</param>
        /// <param name="e"><see cref="PropertyChangedEventArgs"/> of the tracked complex wrapper.</param>
        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChanged))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        /// <summary>
        /// Keeps track of any changes from the original property vl.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value being tracked.</typeparam>
        /// <param name="currentValue">The stored value of the tracked property. </param>
        /// <param name="newValue">The new value of the tracked property.</param>
        /// <param name="propertyName">The name of the property being tracked.</param>
        private void UpdateOriginalValue<TValue>(TValue currentValue, TValue newValue, string propertyName)
        {
            
            // If the changed property is not in the originalValues dictionary, add it.
            if (!originalValues.ContainsKey(propertyName))
            {
                originalValues.Add(propertyName, currentValue);
                DeltaChanges++;
                OnPropertyChanged(nameof(IsChanged));
                OnPropertyChanged(nameof(DeltaChanges));
            }
            // Otherwise, if the property has "returned" to its original value, remove it.
            else
            {
                if (Equals((TValue)originalValues[propertyName], newValue))
                {
                    originalValues.Remove(propertyName);
                    DeltaChanges--;
                    OnPropertyChanged(nameof(IsChanged));
                    OnPropertyChanged(nameof(DeltaChanges));
                }
            }
        }

        private void ValidateFluentErrors()
        {

            // Use the provided FluentValidation validator to generate a list of errors for the updated property.
            ValidationResult validationResult = validator.Validate(Model);

            // Load those errors into the error dictionary of the base class.
            LoadErrors(validationResult);
        }

        private void ValidateDataAnnotations<Tvalue>(Tvalue value, string propertyName)
        {
            var results = new List<DataAnnotations.ValidationResult>();

            var context = new DataAnnotations.ValidationContext(Model)
            {
                MemberName = propertyName
            };

            DataAnnotations.Validator.TryValidateProperty(value, context, results);

            LoadErrors(propertyName, results);
        }

    }
}