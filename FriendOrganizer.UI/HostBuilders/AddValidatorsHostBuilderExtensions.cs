using FluentValidation;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Validator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FriendOrganizer.UI.HostBuilders
{
    public static class AddValidatorsHostBuilderExtensions
    {
        public static IHostBuilder AddValidators(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddSingleton<IValidator<Friend>, FriendValidator>();
                services.AddSingleton<IValidator<FriendPhoneNumber>, PhoneValidator>();
                services.AddSingleton<IValidator<ProgrammingLanguage>, ProgrammingLanguageValidator>();
                services.AddSingleton<IValidator<Meeting>, MeetingValidator>();
                services.AddSingleton<IValidator<Address>, AddressValidator>();
            });

            return host;
        }
    }
}
