using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TestMaster.Wpf.Helpers;

namespace TestMaster.Wpf.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private string _username = "";
    private string _password = "";
    private string _error = "";
    
    public event Action? LoginSuccess;

    public string Username { get => _username; set => Set(ref _username, value); }
    public string Password { get; set; } = ""; // Пароль не зберігаємо у полі з OnPropertyChanged з міркувань безпеки
    public string Error { get => _error; set => Set(ref _error, value); }

    public ICommand LoginCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        LoginCommand = new RelayCommand(async _ => await ExecuteLogin(), _ => !string.IsNullOrWhiteSpace(Username));
    }

    private async Task ExecuteLogin()
    {
        Error = "";
        var (success, message) = await _authService.LoginAsync(Username, Password);
        if (success)
        {
            LoginSuccess?.Invoke();
        }
        else
        {
            Error = message;
        }
    }
}