using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FriendOrganizer.UI.Services
{
    /// <summary>
    /// Provides displays and takes responses from various dialog box options.
    /// </summary>
    public class MessageDialogService : IMessageDialogService
    {
        private static MetroWindow MetroWindow => (MetroWindow)App.Current.MainWindow;

        /// <summary>
        /// Displays a <see cref="MessageDialog"/>
        /// </summary>
        /// <param name="text">The message displayed in the dialog box.</param>
        /// <param name="title">The caption of the dialog box.</param>
        /// <returns>The response made by the user as a <see cref="MessageDialogResult"/> value.</returns>
        public async Task<MessageDialogResult> ShowOKCancelDialogAsync(string title, string text)
        {
            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegative);

            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }

        public async Task ShowInfoDialogAsync(string text) 
        {
            await MetroWindow.ShowMessageAsync("Info", text, MessageDialogStyle.Affirmative);
        }

        public MessageDialogResult ShowOKCancelDialogModal(string title, string text)
        {
            var result = MetroWindow.ShowModalMessageExternal(title, text, MessageDialogStyle.AffirmativeAndNegative);

            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative
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
