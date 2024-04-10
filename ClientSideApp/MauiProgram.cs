using ClientSideApp.Services;
using ClientSideApp.ViewModels;
using ClientSideApp.ViewModels.Startup;
using ClientSideApp.Views;
using ClientSideApp.Views.Startup;
using Microsoft.Extensions.Logging;

namespace ClientSideApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IUserService, UserService>();

            //Views
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddTransient<AdminUsersPage>();
            builder.Services.AddTransient<AdminUserRegistrationPage>();


            //View Models
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddTransient<AdminUsersViewModel>();
            builder.Services.AddTransient<AdminUserRegistrationViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
