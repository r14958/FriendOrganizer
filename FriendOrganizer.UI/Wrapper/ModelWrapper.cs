using FluentValidation;
using FluentValidation.Results;
using DataAnnotations = System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Generic data Model wrapper
    /// </summary>
    /// <typeparam name="T">The type of the model being wrapped.</typeparam>
    public class ModelWrapper<T> : NotifyDataErrorInfoBase
    {
        private readonly IValidator<T> validator;

        public ModelWrapper(T model, IValidator<T> validator=null)
        {
            Model = model;
            this.validator = validator;
        }


        /// <summary>
        /// Gets the class T being wrapped
        /// </summary>
        public T Model { get; }


        /// <summary>
        /// Returns the value of type Tvalue from a property named propertyName of a class instance of type T.
        /// </summary>
        /// <typeparam name="Tvalue">The type of property value being returned.</typeparam>
        /// <param name="propertyName">The name of the property whose value is being returned.</param>
        /// <returns></returns>
        protected virtual Tvalue GetValue<Tvalue>([CallerMemberName] string propertyName = null)
        {
            return (Tvalue)typeof(T).GetProperty(propertyName).GetValue(Model);
        }

        /// <summary>
        /// Sets the value of a property named propertyName with type Tvalue of a class instance of type T.
        /// </summary>
        /// <typeparam name="Tvalue">The type of the property value being set.</typeparam>
        /// <param name="value">The new value of property.</param>
        /// <param name="propertyName">The name of the property being set.</param>
        protected virtual void SetValue<Tvalue>(Tvalue value, 
            [CallerMemberName] string propertyName = null)
        {
            // If the property value did not change, do nothing...
            if (GetValue<Tvalue>(propertyName).Equals(value)) return;
            
            // Update the property of Model with the new value.
            typeof(T).GetProperty(propertyName).SetValue(Model, value);

            // Raise the property changed event from the base class
            OnPropertyChanged(propertyName);

            // Clear any existing validation errors for the property from the errors dictionary in the base class.
            ClearErrors(propertyName);


            // If a validator was provided check for FluentValidation errors.
            if (validator != null) ValidateFluentErrors(propertyName); 
            
            // Then validate based on data annotations.
            ValidateDataAnnotations(value, propertyName);
        }

        private void ValidateFluentErrors(string propertyName)
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