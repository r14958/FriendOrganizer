using FluentValidation;
using FriendOrganizer.DataAccess;
using FriendOrganizer.DataAccess.Services;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Validator;
using FriendOrganizer.UI.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prism.Events;
using System;
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
        }

        /// <summary>
        /// Creates a static <see cref="IHostBuilder"/> that will generate an <see cref="IHost"/> to provide all services to the app.
        /// </summary>
        /// <param name="args">Optional string array, not used.</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateHostBuilder( string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    // ***Don't forget to change this files property settings to "Copy if newer"!***
                    config.AddJsonFile("AppSettings.json");

                })
                .ConfigureServices((context, services) =>
                {
                    // Get the DbContext connection string from the AppSettings json file.
                    string connectionString = context.Configuration.GetConnectionString("sqlite");

                    // Define an Action delegate for the DbContextOptionsBuilder, defining Sqlite as the DB provider, 
                    // along with its connection string.
                    void configureDbContext(DbContextOptionsBuilder builder) => builder.UseSqlite(connectionString);

                    // Register the Prism Event Aggregator
                    services.AddSingleton<IEventAggregator, EventAggregator>();
                    
                    // Register the DbContext with its options.  Note that is only needed to support EFCore migrations.
                    services.AddDbContext<FriendOrganizerDbContext>(configureDbContext);
                    
                    // Register the DbContextFactory with its connection string.  This factory is what actually generates the
                    // DbContext in the app.
                    services.AddSingleton(new FriendOrganizerDbContextFactory(configureDbContext));
                    
                    // Register the Data Services
                    services.AddSingleton<IDataServiceAsync<Friend>, DataServiceAsyncBase<Friend>>();
                    // Constructor for LookupDataServiceAsync<Friend> requires two parameters, so this is how they are provided.
                    services.AddSingleton<ILookupDataServiceAsync<Friend>>(s => 
                        new LookupDataServiceAsync<Friend>(new FriendOrganizerDbContextFactory(configureDbContext), nameof(Friend.FullName)));

                    //Register ViewModels
                    services.AddSingleton<INavigationViewModel, NavigationViewModel>();
                    services.AddSingleton<IFriendDetailViewModel, FriendDetailViewModel>();
                    services.AddSingleton<MainViewModel>();

                    //Register Validators
                    services.AddSingleton<IValidator<Friend>, FriendValidator>();

                    // Register the MainWindow
                    services.AddSingleton<MainWindow>();
                });
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Asynchronously start the hosting service.
            await host.StartAsync();

            // Get an instance of the DbContext factory from the hosting service (DI service).
            FriendOrganizerDbContextFactory contextFactory = host.Services.GetRequiredService<FriendOrganizerDbContextFactory>();

            // Use that factory to creatge an instance of the DbContext.
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
