using FriendOrganizer.DataAccess.Services.Repositories;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using FriendOrganizer.UI.Data;
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
                services.AddTransient<IDataRepository<Friend>, FriendRepository>();
                services.AddTransient<IPhoneNumbersRepository, PhoneNumbersRepository>();
                //services.AddTransient<PhoneNumbersRepository>();
            });

            return host;
        }

    }
}
