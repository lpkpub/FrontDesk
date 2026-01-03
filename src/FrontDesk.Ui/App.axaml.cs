using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FrontDesk.Shared;
using FrontDesk.Ui.Services;
using FrontDesk.Ui.ViewModels;
using FrontDesk.Ui.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;

namespace FrontDesk.Ui;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var serviceProvider = ConfigureServices();
            desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
            var mainMenuViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainMenuViewModel
            };
            desktop.MainWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        // Config
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        serviceCollection.AddSingleton<IConfiguration>(config);

        // HttpClient
        serviceCollection.AddHttpClient<IApiClient, ApiClient>((sp, client) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = new Uri(config["Api:BaseUrl"]!);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var apiKey = config[Const.API_KEY] ?? throw new InvalidOperationException($"{Const.API_KEY} is not set.");
            client.DefaultRequestHeaders.TryAddWithoutValidation(Const.API_KEY_HEADER_NAME, apiKey);
        });

        // UI
        serviceCollection.AddSingleton<MainWindowViewModel>();

        return serviceCollection.BuildServiceProvider();
    }
}