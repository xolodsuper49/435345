using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TestMaster.Wpf.Helpers;

namespace TestMaster.Wpf.ViewModels;

public class SelectableAnswerViewModel : ViewModelBase
{
    public string Text { get; }
    private bool _isSelected;
    public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }
    public bool IsCorrect { get; }

    public SelectableAnswerViewModel(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}

public class TestTakingViewModel : ViewModelBase
{
    private readonly Test _test;
    private readonly IAttemptService _attemptService;
    private readonly ILoggingService _loggingService;
    private readonly IAuthService _authService;
    
    private int _currentQuestionIndex = -1;
    private readonly List<bool> _results = new();

    public event System.Action<string>? TestFinished;

    public string TestTitle => _test.Title;
    private string _currentQuestionText = "";
    public string CurrentQuestionText { get => _currentQuestionText; set => Set(ref _currentQuestionText, value); }
    public ObservableCollection<SelectableAnswerViewModel> CurrentAnswers { get; } = new();
    
    public ICommand NextQuestionCommand { get; }

    public TestTakingViewModel(Test test, IAttemptService attemptService, ILoggingService loggingService, IAuthService authService)
    {
        _test = test;
        _attemptService = attemptService;
        _loggingService = loggingService;
        _authService = authService;
        
        NextQuestionCommand = new RelayCommand(_ => ShowNextQuestion(), _ => CanGoNext());
        ShowNextQuestion();
    }

    private void ShowNextQuestion()
    {
        if (_currentQuestionIndex >= 0)
        {
            var selectedAnswer = CurrentAnswers.FirstOrDefault(a => a.IsSelected);
            _results.Add(selectedAnswer?.IsCorrect ?? false);
        }
        
        _currentQuestionIndex++;
        if (_currentQuestionIndex < _test.Questions.Count)
        {
            var question = _test.Questions.ElementAt(_currentQuestionIndex);
            CurrentQuestionText = $"Питання {_currentQuestionIndex + 1}/{_test.Questions.Count}: {question.Text}";
            CurrentAnswers.Clear();
            foreach (var option in question.Options)
            {
                CurrentAnswers.Add(new SelectableAnswerViewModel(option.Text, option.IsCorrect));
            }
        }
        else
        {
            CalculateResultAndFinish();
        }
    }

    private bool CanGoNext() => CurrentAnswers.Any(a => a.IsSelected);

    private async void CalculateResultAndFinish()
    {
        int correctAnswers = _results.Count(r => r);
        int totalQuestions = _test.Questions.Count;
        double score = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;
        
        var student = _authService.CurrentUser;
        if (student != null)
        {
            await _attemptService.SaveAttemptAsync(_test.Id, student.Id, correctAnswers, totalQuestions);
            await _loggingService.LogAsync($"'{student.UserName}' пройшов '{_test.Title}' з результатом {score:F0}% ({correctAnswers}/{totalQuestions}).");
        }
        
        string resultMessage = $"Тест '{TestTitle}' завершено!\nВаш результат: {correctAnswers} з {totalQuestions} ({score:F0}%)";
        TestFinished?.Invoke(resultMessage);
    }
}