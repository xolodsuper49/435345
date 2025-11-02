using System.Windows;
using TestMaster.Wpf.ViewModels;

namespace TestMaster.Wpf.Views;

public partial class QuestionEditorView : Window
{
    public QuestionEditorView(QuestionEditorViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        
        vm.RequestClose += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
    }
}
