using FriendOrganizer.DataAccess;
using FriendOrganizer.DataAccess.Services.Lookups;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prism.Events;

namespace FriendOrganizer.UI.HostBuilders
{
    public static class AddServicesHostBuilderExtensions
    {
        public static IHostBuilder AddServices(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddSingleton<IMessageDialogService, MessageDialogService>();
                services.AddSingleton<IEventAggregator, EventAggregator>();
            });

            return host;
        }
    }
}
