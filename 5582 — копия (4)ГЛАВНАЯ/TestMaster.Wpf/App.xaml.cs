using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using TestMaster.Wpf.ViewModels;
using TestMaster.Wpf.Views;

namespace TestMaster.Wpf;

public partial class App : System.Windows.Application
{
    private readonly IHost _host;
    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TestMaster.db");
                services.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"), ServiceLifetime.Singleton);

                // Register all services and viewmodels as Singleton for simplicity
                services.AddSingleton<IAuthService, AuthService>();
                services.AddSingleton<IQuestionService, QuestionService>();
                services.AddSingleton<ITestService, TestService>();
                services.AddSingleton<IAttemptService, AttemptService>(); // <-- ДОДАЙТЕ
                services.AddSingleton<ILoggingService, LoggingService>(); // <-- ДОДАЙТЕ

                services.AddSingleton<ShellViewModel>();
                services.AddSingleton<TeacherDashboardViewModel>();
                services.AddSingleton<StudentDashboardViewModel>(); 
                
                // Transient for dialogs and login
                services.AddTransient<LoginViewModel>();
                services.AddTransient<QuestionEditorViewModel>();
                services.AddTransient<TestTakingViewModel>();
                
                // Windows
                services.AddSingleton<MainWindow>();
                services.AddTransient<QuestionEditorView>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        
        var dbContext = _host.Services.GetRequiredService<AppDbContext>();
        await AppDbContext.InitializeDatabaseAsync(dbContext);

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
        base.OnStartup(e);
    }
}
