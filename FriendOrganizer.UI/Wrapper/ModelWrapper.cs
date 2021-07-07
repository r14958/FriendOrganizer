using FluentValidation;
using FluentValidation.Results;
using DataAnnotations = System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Generic data Model wrapper
    /// </summary>
    /// <typeparam name="T">The type of the model being wrapped.</typeparam>
    public abstract class ModelWrapper<T> : NotifyDataErrorInfoBase, IValidatableTrackingObject,
        IValidatableObject
    {
        protected readonly IValidator<T> validator;
        // Dictionary to store the original values of any simple, changed property in
        // the model that is being wrapped by this model wrapper.
        private readonly Dictionary<string, object> originalValues;
        // List of all the complex properties and collections contained in the model
        // that is be wrapped by this model wrapper.
        private readonly List<IValidatableTrackingObject> trackingObjects;

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
            ValidateErrors();
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
        /// Returns whether error tracking dictionary is empty (returns true) or not (returns false).
        /// </summary>
        public bool IsValid
        {
            get
            {
                var isValid = !HasErrors;
                var trackingObjectsValid = trackingObjects.All(t => t.IsValid);
                return isValid && trackingObjectsValid;
            }
        }

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

            // Do the same for each tracking object.
            foreach (var wrapper in trackingObjects)
            {
                wrapper.RejectChanges();
            }
            // Re-validate for errors.
            ValidateErrors();

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
        protected virtual Tvalue GetValueOrDefault<Tvalue>([CallerMemberName] string propertyName = null)
        {
            // If Tvalue is nullable, the result may come back as null.
            var result = typeof(T).GetProperty(propertyName).GetValue(Model);

            // So test for this before attempting a cast. Default covers both nullable and non-nullable types safely.
            return (result == default) ? default : (Tvalue)result;
        }

        /// <summary>
        /// Returns the value of a property of this model wrapper.
        /// </summary>
        /// <typeparam name="Tvalue">The type of property value being returned.</typeparam>
        /// <param name="propertyName">Optional: The name of the property whose value is being returned.
        /// If omitted, the name of the calling method or property will be used.</param>
        /// <returns>The value of the property.</returns>
        //protected virtual Tvalue GetValue<Tvalue>([CallerMemberName] string propertyName = null)
        //{
        //    return (Tvalue)typeof(Tvalue).GetProperty(propertyName).GetValue(Model);
        //}

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
                return GetValueOrDefault<Tvalue>(propertyName);
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
            var currentValue = GetValueOrDefault<Tvalue>(propertyName);

            // If the property value did not change, do nothing...
            if (Equals(currentValue, newValue))
            {
                return;
            }

            //Otherwise, add it or remove it from our originalValues dictionary.
            UpdateOriginalValue(currentValue, newValue, propertyName);

            // Using Reflection, update the property of Model with the new value.
            typeof(T).GetProperty(propertyName).SetValue(Model, newValue);

            // Re-validate for errors after the change.
            ValidateErrors();

            // Raise the property changed event from the base class
            OnPropertyChanged(propertyName);

            // If one exists, also raise one for the "IsChanged" flag for the same property.
            OnPropertyChanged(propertyName + "IsChanged");
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

                ValidateErrors();
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

        private void RegisterTrackingObject(IValidatableTrackingObject trackingObject)
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

            else if (e.PropertyName == nameof(IsValid))
            {
                OnPropertyChanged(nameof(IsValid));
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

        private void ValidateErrors()
        {
            // If a validator was provided check for FluentValidation errors...
            if (validator != null)
            {
                // Use FluentValidation only.
                ValidateFluentErrors();
            }
            else
            {
                //Validate based on data annotations.
                ValidateDataAnnotations();
            }
        }

        // <summary>
        /// Validates the wrapper's model using <see cref="FluentValidation"/> storing any errors in the
        /// base class's <see cref="NotifyDataErrorInfoBase.Errors"/> dictionary.
        /// </summary>
        private void ValidateFluentErrors()
        {
            // Clear out any existing errors.
            ClearAllErrors();

            // Use the FluentValidation validator to generate a list of errors.
            // Note that the fluent validator is currently set to check the errors against the
            // wrapper's model, not the wrapper itself.
            // TODO: Decide if this should be changed.
            FluentValidation.Results.ValidationResult validationResult = validator.Validate(Model);

            // Get the list of validation "failures."
            var results = validationResult.Errors;

            if(results.Any())
            {
                // Generate a list of unique property names with errors (failures).
                var propertyNames = results.Select(f => f.PropertyName).Distinct().ToList();

                // Cycle through the properties with errors...
                foreach (var propertyName in propertyNames)
                {
                    // Generate a list of unique error messages for each property and
                    // store the list in the Errors dictionary under the property's name.
                    // TODO: Decide if .Contains can safely be replaced with ==.
                    Errors[propertyName] = results
                        .Where(f => f.PropertyName.Contains(propertyName))
                        .Select(f => f.ErrorMessage)
                        .Distinct()
                        .ToList();

                    // Trigger the errors changed event for that property.
                    OnErrorsChanged(propertyName);
                }
            }

            // Trigger the  changed event for the wrapper's IsValid property.
            OnPropertyChanged(nameof(IsValid));
        }

        /// <summary>
        /// Validates the wrapper using <see cref="DataAnnotations"/> storing any errors in the
        /// base class's <see cref="NotifyDataErrorInfoBase.Errors"/> dictionary.
        /// </summary>
        private void ValidateDataAnnotations()
        {
            // Clear out any existing errors.
            ClearAllErrors();

            // Define an empty list for validation results.
            var results = new List<DataAnnotations.ValidationResult>();

            // Create a context (set of rules) for the validation.
            // Note that we are validating the entire wrapper instance.
            var context = new DataAnnotations.ValidationContext(this);

            // Validate all annotated properties of the wrapper.
            DataAnnotations.Validator.TryValidateObject(this, context, results, true);

            // If there are any errors...
            if (results.Any())
            {
                // Generate a list of unique property names with errors.
                // Note that the DataAnnotations.Validator stores errors by error message
                // with lists of properties with that error, so a SelectMany function is required.
                var propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();

                // Cycle through the properties with errors...
                foreach (var propertyName in propertyNames)
                {
                    // Generate a list of unique error messages for each property and
                    // store the list in the Errors dictionary under the property's name.
                    // TODO: Decide if .Contains can safely be replaced with ==.
                    Errors[propertyName] = results
                        .Where(r => r.MemberNames.Contains(propertyName))
                        .Select(r => r.ErrorMessage)
                        .Distinct()
                        .ToList();

                    // Trigger the errors changed event for that property.
                    OnErrorsChanged(propertyName);
                }
            }

            // Trigger the  changed event for the wrapper's IsValid property.
            OnPropertyChanged(nameof(IsValid));
        }

        public virtual IEnumerable<DataAnnotations.ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}