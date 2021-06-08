using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.ViewModel;
using FriendOrganizer.UI.ViewModel.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FriendOrganizer.UI.HostBuilders
{
    public static class AddViewModelsHostBuilderExtensions
    {
        public static IHostBuilder AddViewModels(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<MainViewModel>();
                services.AddTransient<NavigationViewModel>();
                services.AddTransient<FriendDetailViewModel>();
                services.AddTransient<MeetingDetailViewModel>();
                services.AddTransient<ProgrammingLanguageDetailViewModel>();

                services.AddTransient<INavigationViewModel, NavigationViewModel>();
                
                services.AddSingleton<CreateViewModel<FriendDetailViewModel>>(services => () => services.GetRequiredService<FriendDetailViewModel>());
                services.AddSingleton<CreateViewModel<MeetingDetailViewModel>>(services => () => services.GetRequiredService<MeetingDetailViewModel>());
                services.AddSingleton<CreateViewModel<NavigationViewModel>>(services => () => services.GetRequiredService<NavigationViewModel>());

                //services.AddSingleton<CreateDetailViewModel>();
                services.AddSingleton<CreateDetailViewModel>(provider => name =>
                {
                    switch (name)
                    {
                        case nameof(FriendDetailViewModel):
                            return provider.GetRequiredService<FriendDetailViewModel>();
                        case nameof(MeetingDetailViewModel):
                            return provider.GetRequiredService<MeetingDetailViewModel>();
                        case nameof(ProgrammingLanguageDetailViewModel):
                            return provider.GetRequiredService<ProgrammingLanguageDetailViewModel>();
                        default:
                            throw new ArgumentException($"Could not locate dependency named {name}.");
                    }

                });
                
                services.AddSingleton<IFriendOrganizerViewModelFactory, FriendOrganizerViewModelFactory>();
            });

            return host;
        }
    }
}
