using CalorieCalculator.Service;
using Microsoft.Extensions.Logging;
using System.Reflection;
using CommunityToolkit.Maui;

namespace CalorieCalculator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<ApiService>();

            RegisterViewModels(builder.Services);
            RegisterPages(builder.Services);

            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<AuthApiService>();

            builder.Logging.AddDebug();

            return builder.Build();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            var viewModels = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.Name.EndsWith("ViewModel"));

            foreach (var viewModel in viewModels)
            {
                services.AddTransient(viewModel);
            }
        }

        private static void RegisterPages(IServiceCollection services)
        {
            var pages = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.IsSubclassOf(typeof(ContentPage)));

            foreach (var page in pages)
            {
                services.AddTransient(page);
            }
        }
    }
}
