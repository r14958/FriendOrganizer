using FriendOrganizer.UI.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DataAnnotations = System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FriendOrganizer.UI.Wrapper
{
    public class NotifyDataErrorInfoBase : NotifyPropChangedBase, INotifyDataErrorInfo
    {

        /// <summary>
        /// A dictionary to hold the model's errors.
        /// Key: the name of the property.
        /// Value: List of errors (strings).
        /// </summary>
        protected Dictionary<string, List<string>> Errors = new();
        private static int deltaChanges;


        // If the errors dictionary has entries, then the model has errors.
        public bool HasErrors => Errors.Any();

        public static bool HasAnyChanges => DeltaChanges > 0;

        protected static int DeltaChanges { get => deltaChanges; set => deltaChanges = value; }


        // Event to notify that the model's error dictionary has changed.
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Returns all errors associated with a model property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>A list of strings (error descriptions) or null if there are no errors.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            // If the errors dictionary contains the propertyName
            return Errors.ContainsKey(propertyName)
                // Return its list of errors
                ? Errors[propertyName]
                // or return null.
                : null;
        }

        /// <summary>
        /// Invokes the event handler when the validation errors for a particular property have changed.
        /// </summary>
        /// <param name="propertyName">The name of the property whose entries in the <see cref="Errors"/> errors dictionary are being changed.</param>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            // Raise the property changed event of the NotifyPropChangedBase class for the HasErrorProperty.
            base.OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// Adds a single error message to the <see cref="Errors"/> dictionary.
        /// </summary>
        /// <param name="error"> The validation error object.</param>
        private void AddError(DataAnnotations.ValidationResult error)
        {

            foreach (var propertyName in error.MemberNames.ToList())
            {
                // If the property is not found in the error dictionary...
                if (!Errors.ContainsKey(propertyName))
                {
                    // Add it with a blank list of error messages.
                    Errors[propertyName] = new List<string>();

                }
                // If the error message is not in the list for that property...
                if (!Errors[propertyName].Contains(error.ErrorMessage))
                {
                    // Add it.
                    Errors[propertyName].Add(error.ErrorMessage);
                    OnErrorsChanged(propertyName);
                } 
            }
        }

        /// <summary>
        /// Removes all entries in the <see cref="Errors"/> dictionary, notifying each 
        /// property that its error status has changed.
        /// </summary>
        protected void ClearAllErrors()
        {
            // Cycle through each key (property name) in the Error dictionary.
            // Note we are copying the keys to a list to a void runtime errors
            // as we delete them.
            foreach (var propertyName in Errors.Keys.ToList())
            {
                Errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
