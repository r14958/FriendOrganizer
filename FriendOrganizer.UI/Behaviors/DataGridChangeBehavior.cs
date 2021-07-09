using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FriendOrganizer.UI.Behaviors
{
    public class DataGridChangeBehavior : ChangeBehavior
    {
        
        private new static void OnErrorAdornerPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid  && GetIsActive(dataGrid))
            {
                DataGrid_Loaded(dataGrid, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Callback for the <see cref="IsActiveProperty"/> change event of the <see cref="DataGrid"/> <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>, expected to be a <see cref="DataGrid"/></param>
        /// <param name="e"></param>
        private new static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Only if a DataGrid object was passed in...
            if (d is DataGrid dataGrid)
            { 
                // If value changed to True...
                if ((bool)e.NewValue)
                {
                    // Set the event handler.
                    dataGrid.Loaded += DataGrid_Loaded;
                }
                // If value changed to False...
                else
                {
                    // Remove the event handler.
                    dataGrid.Loaded -= DataGrid_Loaded;
                }
            }
        }

        /// <summary>
        /// Event handler for the Loaded event of the <see cref="DataGrid"/> <see cref="DependencyObject"/>.
        /// Resets the style bindings for each <see cref="DataGridTextColumn"/> based on their original path.
        /// Binding is reset for both the <see cref="DataGridTextColumn.DefaultElementStyle"/> and <see cref="DataGridTextColumn.DefaultEditingElementStyle"/>. 
        /// </summary>
        /// <param name="sender">The <see cref="DataGrid"/> object.</param>
        /// <param name="e"></param>
        private static void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            foreach (var textColumn in dataGrid.Columns.OfType<DataGridTextColumn>())
            {
                var binding = textColumn.Binding as Binding;
                if (binding != null)
                {
                    textColumn.EditingElementStyle
                        = CreateEditingElementStyle(dataGrid, binding.Path.Path);
                    textColumn.ElementStyle
                        = CreateElementStyle(dataGrid, binding.Path.Path);
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="DataGridTextColumn.DefaultElementStyle"/> of the <see cref="TextBlock"/> 
        /// of the <see cref="DataGridTextColumn"/>, based on the key "TextBlockBaseStyle".
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGrid"/> object.</param>
        /// <param name="bindingPath">The path (string) of the binding for the <see cref="TextBlock"/> of the <see cref="DataGridTextColumn"/>.</param>
        /// <returns>The <see cref="Style"/> defined in the app's resources.</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private static Style CreateElementStyle(DataGrid dataGrid, string bindingPath)
        {
            // Get the base style for the TextBlock.  Will throw exception if the key is not found.
            var baseStyle = dataGrid.FindResource("TextBlockBaseStyle") as Style;
            
            // Create a new instance of this style.
            var style = new Style(typeof(TextBlock), baseStyle);

            // Add style setters for for the non-Editing form of the DataGrid text column (TextBlock).
            AddSetters(style, bindingPath, dataGrid);
            
            // Return the style so it can be set.
            return style;
        }

        /// <summary>
        /// Sets the <see cref="DataGridTextColumn.DefaultEditingElementStyle"/> of the <see cref="TextBox"/> 
        /// of the <see cref="DataGridTextColumn"/>, based on the key app's default style for <see cref="TextBox"/>.
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGrid"/> object.</param>
        /// <param name="bindingPath">The path (string) of the binding for the <see cref="TextBox"/> of the <see cref="DataGridTextColumn"/>.</param>
        /// <returns>The <see cref="Style"/> defined as the app's default for <see cref="TextBox"/>.</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private static Style CreateEditingElementStyle(DataGrid dataGrid, string bindingPath)
        {
            var baseStyle = dataGrid.FindResource(typeof(TextBox)) as Style;
            var style = new Style(typeof(TextBox), baseStyle);
            AddSetters(style, bindingPath, dataGrid);
            return style;
        }

        /// <summary>
        /// Adds style setters to three attached properties:
        /// 1) <see cref="ChangeBehavior.IsActiveProperty"/>, default is false.
        /// 2) <see cref="ChangeBehavior.IsChangedProperty"/>.
        /// 3) <see cref="ChangeBehavior.OriginalValueProperty"/>
        /// </summary>
        /// <param name="style">The style that will be set.</param>
        /// <param name="bindingPath">The binding path for the <see cref="ChangeBehavior.IsActiveProperty"/>.
        /// This will be used to define the binding path for the other two attached properties.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static void AddSetters(Style style, string bindingPath, DataGrid dataGrid)
        {
            style.Setters.Add(new Setter(ChangeBehavior.IsActiveProperty, false));
            style.Setters.Add(new Setter(ChangeBehavior.IsChangedProperty, 
                new Binding(bindingPath + "IsChanged")));
            style.Setters.Add(new Setter(ChangeBehavior.OriginalValueProperty,
                new Binding(bindingPath + "OriginalValue")));

            
            UpdateStyleWithValidationErrorTemplate(dataGrid, ref style);
        }
    }
}
