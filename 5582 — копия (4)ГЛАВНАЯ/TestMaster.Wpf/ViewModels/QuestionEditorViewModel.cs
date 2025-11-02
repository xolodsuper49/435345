using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TestMaster.Wpf.Helpers;

namespace TestMaster.Wpf.ViewModels;

public class AnswerOptionViewModel : ViewModelBase
{
    private string _text = "";
    public string Text { get => _text; set => Set(ref _text, value); }
    private bool _isCorrect;
    public bool IsCorrect { get => _isCorrect; set => Set(ref _isCorrect, value); }
}

public class QuestionEditorViewModel : ViewModelBase
{
    public event EventHandler<bool>? RequestClose;

    private string _questionText = "Нове запитання";
    public string QuestionText { get => _questionText; set => Set(ref _questionText, value); }
    
    public ObservableCollection<AnswerOptionViewModel> AnswerOptions { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AddOptionCommand { get; }
    public ICommand RemoveOptionCommand { get; }

    public QuestionEditorViewModel()
    {
        SaveCommand = new RelayCommand(_ => RequestClose?.Invoke(this, true), _ => AnswerOptions.Count(o => o.IsCorrect) == 1);
        CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(this, false));
        AddOptionCommand = new RelayCommand(_ => AnswerOptions.Add(new AnswerOptionViewModel { Text = "Новий варіант" }));
        RemoveOptionCommand = new RelayCommand(param => { if (param is AnswerOptionViewModel o) AnswerOptions.Remove(o); });
        
        AnswerOptions.Add(new AnswerOptionViewModel { Text = "Варіант А", IsCorrect = true });
        AnswerOptions.Add(new AnswerOptionViewModel { Text = "Варіант Б" });
    }
    
    public void LoadQuestion(Question question)
    {
        QuestionText = question.Text;
        AnswerOptions.Clear();
        foreach (var option in question.Options)
        {
            AnswerOptions.Add(new AnswerOptionViewModel { Text = option.Text, IsCorrect = option.IsCorrect });
        }
    }
}