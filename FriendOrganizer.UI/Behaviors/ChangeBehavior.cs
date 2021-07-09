using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Syncfusion.Windows.Shared;

namespace FriendOrganizer.UI.Behaviors
{
    /// <summary>
    /// Defines custom behaviors on attached properties for <see cref="Control"/> dependency objects.
    /// Note that <see cref="DataGrid"/> is not on the list, as it gets its own <see cref="DataGridChangeBehavior"/> class.
    /// Also, <see cref="TextBlock"/> is not on the lits, because it is not of type <see cref="Control"/> and
    /// its style is defined separately in <see cref="FriendOrganizer.UI.Styles.ControlBaseStyle.xaml"/>.
    /// </summary>
    public class ChangeBehavior
    {
        // Dictionary to store types (key) and their associated dependency properties to assign custom behaviors.
        private static readonly Dictionary<Type, DependencyProperty> defaultProperties;
        public enum ErrorAdornerPositions
        {
            InsideRight,
            InsideLeft,
            InsideCenter,
            Above,
            Below,
            ToLeft,
            ToRight
        }

        static ChangeBehavior()
        {
            // Populate dictionary of controls to assign these custom behaviors.
            defaultProperties = new()
            {
                [typeof(TextBox)] = TextBox.TextProperty,
                [typeof(CheckBox)] = ToggleButton.IsCheckedProperty,
                [typeof(ComboBox)] = Selector.SelectedValueProperty,
                [typeof(DateTimeEdit)] = DateTimeEdit.DateTimeProperty,

            };
        }

        /// <summary>
        /// Gets the attached <see cref="IsActiveProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <returns>True or False.</returns>
        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsActiveProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="IsActiveProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <param name="value">The bool value being set.</param>
        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        /// <summary>
        /// Identifies the attached property <see cref="IsActiveProperty"/> for the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached("IsActive", 
                typeof(bool), 
                typeof(ChangeBehavior), 
                new PropertyMetadata(false, OnIsActivePropertyChanged));

        /// <summary>
        /// Gets the attached <see cref="IsChangedProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <returns>True or False.</returns>
        public static bool GetIsChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsChangedProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="IsChangedProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <param name="value">The bool value being set.</param>
        public static void SetIsChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(IsChangedProperty, value);
        }

        /// <summary>
        /// Identifies the attached property <see cref="IsChangedProperty"/> for the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        public static readonly DependencyProperty IsChangedProperty =
            DependencyProperty.RegisterAttached("IsChanged", 
                typeof(bool), 
                typeof(ChangeBehavior), 
                new PropertyMetadata(false));

        /// <summary>
        /// Gets the attached <see cref="OriginalValueProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <returns>The value of the attached property.  May be null.</returns>
        public static object GetOriginalValue(DependencyObject obj)
        {
            return (object)obj.GetValue(OriginalValueProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="OriginalValueProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <param name="value">The value being set.</param>
        public static void SetOriginalValue(DependencyObject obj, object value)
        {
            obj.SetValue(OriginalValueProperty, value);
        }

        /// <summary>
        /// Identifies the attached property <see cref="OriginalValueProperty"/> for the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// Default value is null./>
        /// </summary>
        public static readonly DependencyProperty OriginalValueProperty =
            DependencyProperty.RegisterAttached("OriginalValue", 
                typeof(object),
                typeof(ChangeBehavior), 
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the attached <see cref="OriginalValueConverterProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <returns>The value of the attached property.  Will be null if no converter is provided or required.</returns>
        public static IValueConverter GetOriginalValueConverter(DependencyObject obj)
        {
            return (IValueConverter)obj.GetValue(OriginalValueConverterProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="OriginalValueConverterProperty"/> of the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>, expected to be a <see cref="Control"/>.</param>
        /// <param name="value">The value being set of type <see cref="IValueConverter"/> or null.</param>
        public static void SetOriginalValueConverter(DependencyObject obj, IValueConverter value)
        {
            obj.SetValue(OriginalValueConverterProperty, value);
        }

        /// <summary>
        /// Identifies the attached property <see cref="OriginalValueConverterProperty"/> for the <see cref="Control"/> <see cref="DependencyObject"/>.
        /// Default value is null.  Property changed callback is <see cref="OnOriginalValueConverterPropertyChanged(DependencyObject, DependencyPropertyChangedEventArgs)"/>
        /// </summary>
        public static readonly DependencyProperty OriginalValueConverterProperty =
            DependencyProperty.RegisterAttached("OriginalValueConverter", 
                typeof(IValueConverter),
                typeof(ChangeBehavior), 
                new PropertyMetadata(null, OnOriginalValueConverterPropertyChanged));

        public static ErrorAdornerPositions GetErrorAdornerPosition(DependencyObject obj)
        {
            return (ErrorAdornerPositions)obj.GetValue(ErrorAdornerPositionProperty);
        }

        public static void SetErrorAdornerPosition(DependencyObject obj, ErrorAdornerPositions value)
        {
            obj.SetValue(ErrorAdornerPositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for ErrorAdornerPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorAdornerPositionProperty =
            DependencyProperty.RegisterAttached("ErrorAdornerPosition",
                typeof(ErrorAdornerPositions),
                typeof(DataGridChangeBehavior),
                new PropertyMetadata(ErrorAdornerPositions.InsideRight, OnErrorAdornerPositionChanged));

        /// <summary>
        /// Event handler for the <see cref="IsActiveProperty"/> changed event.
        /// Creates or deletes bindings for the attached properties <see cref="IsChangedProperty"/> and <see cref="OriginalValueProperty"/>,
        /// depending on the <see cref="IsActiveProperty"/> of the <see cref="Control"/>.
        /// </summary>
        /// <param name="d">The <see cref="Control"/> whose <see cref="IsActiveProperty"/> is being set.</param>
        /// <param name="e">Change event args for the <see cref="IsActiveProperty"/>. </param>
        protected static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Get the type of the dep. obj. (TextBox, ComboBox, etc.)
            Type type = d.GetType();

            // If the dp object is in the dictionary...
            if (defaultProperties.ContainsKey(type))
            {
                // Lookup that dp objects default property.
                var defaultProperty = defaultProperties[type];

                // If the "IsActive" dp is set to true...
                if ((bool)e.NewValue)
                {
                    // Grab the binding for the default property on the dp object
                    var binding = BindingOperations.GetBinding(d, defaultProperty);

                    // Make sure the dp object has a binding...
                    if (binding != null)
                    {
                        // Get the path strings from the binding.
                        // That string will be in the format "Person.FirstName"
                        string bindingPath = binding.Path.Path;

                        // Create a new Binding for the "IsChanged" version of
                        // the default property (e.g., Person.FirstNameIsChanged).
                        // and set it to the IsChanged attached property.
                        BindingOperations.SetBinding(d, IsChangedProperty,
                            new Binding(bindingPath + "IsChanged")
                            {
                                Mode = BindingMode.OneWay
                            });

                        // For the original value property, a value converter may be 
                        // needed to store this property in the correct format.
                        CreateOriginalValueBinding(d, bindingPath + "OriginalValue");
                        
                        
                        SetValidationErrorTemplate(d);
                    }
                }
                else // IsActive is false, clear the bindings for the attached properties...
                {
                    BindingOperations.ClearBinding(d, IsChangedProperty);
                    BindingOperations.ClearBinding(d, OriginalValueProperty);
                }
            }
        }

        private static void SetValidationErrorTemplate(DependencyObject d)
        {

            // Do nothing if the provided dp is not a Framework Element.
            if (d is not FrameworkElement element) return;
            
            Type type = d.GetType();
                
            // Since element's Style property is immutable, a copy must be made.
            var baseStyle = element.FindResource(type) as Style;
            var style = new Style(type, baseStyle);

            // Copy any existing setters
            foreach (var setter in baseStyle.Setters)
            {
                style.Setters.Add(setter);
            }

            // Add the Validation Error Template to Style.
            UpdateStyleWithValidationErrorTemplate(element, ref style);

            // Replace element's style with the new style.
            element.Style = style; 

        }

        /// <summary>
        /// Uses the attached property <see cref="GetErrorAdornerPosition"/> to select the proper <see cref="Validation.ErrorTemplateProperty"/>
        /// and add it to inputted <see cref="Style"/>.
        /// </summary>
        /// <param name="element">The <see cref="DependencyObject"/> that provides the base style to receive the <see cref="Validation.ErrorTemplateProperty"/>.</param>
        /// <param name="style">The incoming style to receive the <see cref="Validation.ErrorTemplateProperty"/>.</param>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        protected static void UpdateStyleWithValidationErrorTemplate(FrameworkElement element, ref Style style)
        {
            // Use the attached property to get the proper resourceKey for the ErrorTemplate.
            string resourceKey = GetErrorTemplateResourceKey(GetErrorAdornerPosition(element));
            
            // Use the resourceKey at its target element to find the ErrorTemplate.
            var resource = element.FindResource(resourceKey);

            // Modify the inputted style by adding the requested validation error template.
            style.Setters.Add(new Setter(Validation.ErrorTemplateProperty, resource));
        }

        /// <summary>
        /// Re-initializes the binding of a <see cref="OriginalValueProperty"/> of a <see cref="Control"/> that utilizes a <see cref="IValueConverter"/>.
        /// </summary>
        /// <param name="d">The <see cref="Control"/> whose <see cref="OriginalValueProperty"/> requires a value converter. </param>
        /// <param name="e"></param>
        /// <remarks>When a <see cref="Control"/>'s attached <see cref="IsActiveProperty"/> is set to true using an implicit style 
        /// (rather than directly on the framework element), and its 
        /// <see cref="OriginalValueProperty"/> requires a <see cref="IValueConverter"/>, that value converter will be null.  So when the <see cref="IsChangedProperty"/>
        /// changes to true, the original value will be stored without any conversion.  This method resets the binding and its value converter 
        /// when it is first called.</remarks>
        private static void OnOriginalValueConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Make sure the binding is retrievable and if it is, grab it.
            if (BindingOperations.GetBinding(d, OriginalValueProperty) is Binding originalValueBinding)
            {
                // Reset the binding to the control, using its original path.
                CreateOriginalValueBinding(d, originalValueBinding.Path.Path);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Binding"/> for the <see cref="OriginalValueProperty"/> for a <see cref="Control"/>
        /// based on the provided <see cref="Binding.Path"/> path string.
        /// </summary>
        /// <param name="d">The <see cref="Control"/> which has an attached <see cref="OriginalValueProperty"/>.</param>
        /// <param name="originalValueBindingPath">Path string for the new <see cref="Binding"/>.</param>
        private static void CreateOriginalValueBinding(DependencyObject d, string originalValueBindingPath)
        {
            var newBinding = new Binding(originalValueBindingPath)
            {
                // Get the needed value converter (will be null if there is none).
                Converter = GetOriginalValueConverter(d),
                
                // In case the value converter requires it, pass the Control through the CoverterParameter property of the binding.
                ConverterParameter = d,

                // Binding must be one way
                Mode = BindingMode.OneWay
            };

            // Set the new binding on the OriginalValueProperty of the framework element.
            BindingOperations.SetBinding(d, OriginalValueProperty, newBinding);
        }


        protected static void OnErrorAdornerPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetValidationErrorTemplate(d);
        }

        protected static string GetErrorTemplateResourceKey(ErrorAdornerPositions position)
        {
            switch (position)
            {
                case ErrorAdornerPositions.InsideRight:
                    return "DefaultErrorTemplate";
                case ErrorAdornerPositions.InsideLeft:
                    return "AdornerWithinLeftErrorTemplate";
                case ErrorAdornerPositions.InsideCenter:
                    return "AdornerWithinCenterErrorTemplate";
                case ErrorAdornerPositions.Above:
                    return "AdornerAboveErrorTemplate";
                case ErrorAdornerPositions.Below:
                    return "AdornerBelowErrorTemplate";
                case ErrorAdornerPositions.ToLeft:
                    return "AdornerLeftErrorTemplate";
                case ErrorAdornerPositions.ToRight:
                    return "AdornerRightErrorTemplate";
                default:
                    return "DefaultErrorTemplate";
            }
        }

    }
}
