using System.Windows;
using TestMaster.Wpf.ViewModels;
namespace TestMaster.Wpf.Views;
public partial class MainWindow : Window
{
    public MainWindow(ShellViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
