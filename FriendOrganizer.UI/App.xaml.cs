using FriendOrganizer.DataAccess;
using FriendOrganizer.DataAccess.Migrations;
using FriendOrganizer.UI.HostBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace FriendOrganizer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            // Create the static IHost to provide all services for the app.
            host = CreateHostBuilder().Build();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDUyOTMzQDMxMzkyZTMxMmUzMElhcUVQWVd4T2lsYVJXenBtaEYyYk1NTGovTzl2Q2tSOGRZREpXbkFTVHc9");
        }

        /// <summary>
        /// Creates a static <see cref="IHostBuilder"/> that will generate an <see cref="IHost"/> to provide all services to the app.
        /// </summary>
        /// <param name="args">Optional string array, not used.</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateHostBuilder( string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .AddConfiguration()
                .AddDbContext()
                .AddServices()
                .AddDataRepositories()
                .AddValidators()
                .AddViewModels()
                .AddViews();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Asynchronously start the hosting service.
            await host.StartAsync();

            // Get an instance of the DbContext factory from the hosting service (DI service).
            FriendOrganizerDbContextFactory contextFactory = host.Services.GetRequiredService<FriendOrganizerDbContextFactory>();

            // Use that factory to create an instance of the DbContext.
            // If the target DB does not exist, create it and run migrations.
            // If the DB does exists, run any needed migrations.
            // If the DB exists and is current with all migrations, do nothing.
            using(FriendOrganizerDbContext context = contextFactory.CreateDbContext())
            {
                context.Database.Migrate();
            }
            
            // Get an instance of the MainWindow from our hosting service and show it.
            MainWindow mainWindow = host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            // Gracefully shut down the hosting service and dispose of it.
            await host.StopAsync();
            host.Dispose();
            
            base.OnExit(e);
        }

    }
}
