using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FriendOrganizer.UI.Services
{
    /// <summary>
    /// Provides displays and takes responses from various dialog box options.
    /// </summary>
    public class MessageDialogService : IMessageDialogService
    {
        /// <summary>
        /// Displays a standard Windows <see cref="MessageBox"/> of type <see cref="MessageBoxButton.OKCancel"/>.
        /// </summary>
        /// <param name="text">The message displayed in the dialog box.</param>
        /// <param name="title">The caption of the dialog box.</param>
        /// <returns>The response made by the user as a <see cref="MessageDialogResult"/> value.</returns>
        public MessageDialogResult ShowOKCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);

            return result == MessageBoxResult.OK
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }
    }

    public enum MessageDialogResult
    {
        OK,
        Cancel
    }
}
