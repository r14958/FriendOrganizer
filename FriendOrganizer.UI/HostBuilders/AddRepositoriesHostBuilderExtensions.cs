using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FriendOrganizer.UI.HostBuilders
{
    public static class AddRepositoriesHostBuilderExtensions
    {
        public static IHostBuilder AddDataRepositories(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<IFriendRepository, FriendRepository>();
                services.AddTransient<IMeetingRepository, MeetingRepository>();
                services.AddTransient<IProgrammingLanguageRepository, ProgrammingLanguageRepository>();
            });

            return host;
        }

    }
}
