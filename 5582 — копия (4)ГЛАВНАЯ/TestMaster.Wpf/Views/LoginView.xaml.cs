using System.Windows;
using System.Windows.Controls;
using TestMaster.Wpf.ViewModels;

namespace TestMaster.Wpf.Views;
public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        // This is a common pattern to handle PasswordBox, which is not dependency property
        PasswordBox.PasswordChanged += OnPasswordChanged;
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
