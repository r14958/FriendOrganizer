using FriendOrganizer.UI.ViewModel;
using MahApps.Metro.Controls;
using System.Windows;

namespace FriendOrganizer.UI
{

    public partial class MainWindow : MetroWindow
    {
        private readonly MainViewModel viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Private helper method which calls the load method of the viewModel.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        /// <remarks>Keeps the <see cref="MainViewModel.LoadAsync>"/> method out of the constructor. </remarks>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }
    }
}
