using FriendOrganizer.DataAccess;
using FriendOrganizer.DataAccess.Services.Lookups;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.HostBuilders
{
    public static class AddDbContextHostBuilderExtensions
    {
        public static IHostBuilder AddDbContext(this IHostBuilder host)
        {
            host.ConfigureServices((context, services) =>
            {
                // Get the DbContext connection string from the AppSettings json file.
                string connectionString = context.Configuration.GetConnectionString("sqlite");

                // Define an Action delegate for the DbContextOptionsBuilder, defining Sqlite as the DB provider, 
                // along with its connection string.
                void configureDbContext(DbContextOptionsBuilder builder) => builder.UseSqlite(connectionString);

                // Register the DbContext with its options.  Note that is only needed to support EFCore migrations.
                services.AddDbContext<FriendOrganizerDbContext>(configureDbContext);

                services.AddSingleton(new FriendOrganizerDbContextFactory(configureDbContext));

                // Register the Data Services
                // Constructor for LookupDataServiceAsync<Friend> requires two parameters, so this is how they are provided.
                services.AddSingleton<ILookupDataService<Friend>>(s =>
                    new LookupDataServiceAsync<Friend>(new FriendOrganizerDbContextFactory(configureDbContext), nameof(Friend.FullName)));
                // Same for the LookupDataServiceAsync<ProgrammingLanguage>
                services.AddSingleton<ILookupDataService<ProgrammingLanguage>>(s =>
                    new LookupDataServiceAsync<ProgrammingLanguage>(new FriendOrganizerDbContextFactory(configureDbContext), nameof(ProgrammingLanguage.Name)));
                // Same for the LookupDataServiceAsync<ProgrammingLanguage>
                services.AddSingleton<ILookupDataService<Meeting>>(s =>
                    new LookupDataServiceAsync<Meeting>(new FriendOrganizerDbContextFactory(configureDbContext), nameof(Meeting.Title)));
            });

            return host;
        }
    }
}
