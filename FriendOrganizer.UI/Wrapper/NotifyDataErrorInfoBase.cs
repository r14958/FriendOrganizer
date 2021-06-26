using FluentValidation;
using FluentValidation.Results;
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
        protected Dictionary<string, List<string>> errors = new();
        private static int deltaChanges;


        // If the errors dictionary has entries, then the model has errors.
        public bool HasErrors => errors.Any();

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
            return errors.ContainsKey(propertyName)
                // Return its list of errors
                ? errors[propertyName]
                // or return null.
                : null;
        }

        /// <summary>
        /// Invokes the event handler when the validation errors for a particular property have changed.
        /// </summary>
        /// <param name="propertyName">The name of the property whose entries in the <see cref="errors"/> errors dictionary are being changed.</param>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            // Raise the property changed event of the ViewModelBase class for the HasErrorProperty.
            base.OnPropertyChanged(nameof(HasErrors));
        }


        /// <summary>
        /// Loads validation errors from a collection of <see cref="DataAnnotations.ValidationResult"/> to the <see cref="errors"/> error dictionary.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="validationResults"></param>
        protected void LoadErrors(string propertyName, ICollection<DataAnnotations.ValidationResult> validationResults)
        {
            foreach (var result in validationResults)
            {
                ValidationFailure error = new(propertyName, result.ErrorMessage);

                AddError(error);
            }
        }

        
        /// <summary>
        /// Loads errors from the <see cref="ValidationResult"/> to the <see cref="errors"/> error dictionary.
        /// </summary>
        /// <param name="validationResult"></param>
        protected void LoadErrors(ValidationResult validationResult)
        {
            // If there are validation errors...
            if (validationResult != null)
            {
                // Attempt to add them to the error dictionary.
                AddErrors(validationResult);
            }

        }

        /// <summary>
        /// Cycles through all validation errors and adds them to the <see cref="errors"/> error dictionary.
        /// </summary>
        /// <param name="validationResult"></param>
        private void AddErrors(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                AddError(error);
            }
        }

        /// <summary>
        /// Adds a single error message to the <see cref="errors"/> dictionary.
        /// </summary>
        /// <param name="error"> The validation error object.</param>
        private void AddError(ValidationFailure error)
        {
            string propertyName = error.PropertyName;

            // If the property is not found in the error dictionary...
            if (!errors.ContainsKey(propertyName))
            {
                // Add it with a blank list of error messages.
                errors[propertyName] = new List<string>();

            }
            // If the error message is not in the list for that property...
            if (!errors[propertyName].Contains(error.ErrorMessage))
            {
                // Add it.
                errors[propertyName].Add(error.ErrorMessage);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Removes all entries in the <see cref="errors"/> error dictionary for 
        /// a particular property.
        /// </summary>
        /// <param name="propertyName">The name of the property to be removed from the error dictionary.</param>
        protected void ClearErrors(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

    }
}
