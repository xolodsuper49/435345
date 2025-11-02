using Microsoft.Extensions.DependencyInjection;
using System;
using TestMaster.Domain.Enums;
using TestMaster.Wpf.Helpers;

namespace TestMaster.Wpf.ViewModels;

public class ShellViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthService _authService;
    private object? _currentViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => Set(ref _currentViewModel, value);
    }

    public ShellViewModel(IServiceProvider serviceProvider, IAuthService authService)
    {
        _serviceProvider = serviceProvider;
        _authService = authService;
        ShowLoginView();
    }

    private void ShowLoginView()
    {
        var loginVm = _serviceProvider.GetRequiredService<LoginViewModel>();
        loginVm.LoginSuccess += OnLoginSuccess;
        CurrentViewModel = loginVm;
    }

    private void OnLoginSuccess()
    {
        if (CurrentViewModel is LoginViewModel loginVm)
        {
            loginVm.LoginSuccess -= OnLoginSuccess;
        }

        if (_authService.CurrentUser?.Role == Role.Teacher)
        {
            var teacherVm = _serviceProvider.GetRequiredService<TeacherDashboardViewModel>();
            teacherVm.LogoutRequested += OnLogout;
            CurrentViewModel = teacherVm;
        }
        else if (_authService.CurrentUser?.Role == Role.Student) // <-- ДОДАЛИ БЛОК ДЛЯ СТУДЕНТА
        {
            var studentVm = _serviceProvider.GetRequiredService<StudentDashboardViewModel>();
            studentVm.LogoutRequested += OnLogout;
            CurrentViewModel = studentVm;
        }
    }
    
    private void OnLogout()
    {
        if (CurrentViewModel is TeacherDashboardViewModel teacherVm)
        {
            teacherVm.LogoutRequested -= OnLogout;
        }
        else if (CurrentViewModel is StudentDashboardViewModel studentVm) // <-- ДОДАЛИ
        {
            studentVm.LogoutRequested -= OnLogout;
        }
        _authService.Logout();
        ShowLoginView();
    }
}