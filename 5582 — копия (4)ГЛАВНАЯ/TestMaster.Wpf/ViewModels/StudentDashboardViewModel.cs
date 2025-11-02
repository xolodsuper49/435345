using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestMaster.Wpf.Helpers;

namespace TestMaster.Wpf.ViewModels;

public class StudentDashboardViewModel : ViewModelBase
{
    private readonly ITestService _testService;
    private readonly IAuthService _authService;
    private readonly IAttemptService _attemptService; // <-- Поле для сервісу
    private readonly ShellViewModel _shellViewModel;
    private readonly IServiceProvider _serviceProvider;
    
    public event Action? LogoutRequested;
    public ICommand LogoutCommand { get; }
    public ICommand TakeTestCommand { get; }

    public ObservableCollection<Test> AssignedTests { get; } = new();
    public ObservableCollection<Attempt> Attempts { get; } = new();

    private Test? _selectedTest;
    public Test? SelectedTest { get => _selectedTest; set => Set(ref _selectedTest, value); }

    public StudentDashboardViewModel(ITestService testService, IAuthService authService, IAttemptService attemptService, ShellViewModel shellViewModel, IServiceProvider serviceProvider)
    {
        _testService = testService;
        _authService = authService;
        _attemptService = attemptService; // <-- Зберігаємо сервіс
        _shellViewModel = shellViewModel;
        _serviceProvider = serviceProvider;
        
        LogoutCommand = new RelayCommand(_ => LogoutRequested?.Invoke());
        TakeTestCommand = new RelayCommand(_ => StartTest(), _ => SelectedTest != null);
        
        _ = LoadTests();
        _ = LoadAttempts();
    }

    private async Task LoadTests()
    {
        AssignedTests.Clear();
        if (_authService.CurrentUser == null) return;
        
        var tests = await _testService.GetAssignedTestsForStudentAsync(_authService.CurrentUser.Id);
        foreach (var test in tests)
        {
            AssignedTests.Add(test);
        }
    }

    private async Task LoadAttempts()
    {
        Attempts.Clear();
        if (_authService.CurrentUser == null) return;
        var attempts = await _attemptService.GetAttemptsForStudentAsync(_authService.CurrentUser.Id);
        foreach(var attempt in attempts)
        {
            Attempts.Add(attempt);
        }
    }

    private void StartTest()
    {
        if (SelectedTest == null) return;

        var attemptService = _serviceProvider.GetRequiredService<IAttemptService>();
        var loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
        var testVm = new TestTakingViewModel(SelectedTest, attemptService, loggingService, _authService);
        
        testVm.TestFinished += async (message) =>
        {
            MessageBox.Show(message);
            _shellViewModel.CurrentViewModel = this;
            await LoadAttempts(); // Оновлюємо результати
        };
        
        _shellViewModel.CurrentViewModel = testVm;
    }
}