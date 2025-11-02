using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestMaster.Wpf.Helpers;
using TestMaster.Wpf.Views;

namespace TestMaster.Wpf.ViewModels;

// Ця маленька ViewModel обгортає нашу модель Тесту,
// щоб додати логіку збереження при зміні назви.
public class TestViewModel : ViewModelBase
{
    private readonly Test _test;
    private readonly ITestService _testService;

    public Guid Id => _test.Id;
    public ObservableCollection<Question> Questions { get; set; }

    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            if (Set(ref _title, value) && !string.IsNullOrWhiteSpace(value))
            {
                // Зберігаємо зміни в базі даних "на льоту"
                _ = _testService.UpdateTestTitleAsync(Id, value);
            }
        }
    }

    public TestViewModel(Test test, ITestService testService)
    {
        _test = test;
        _testService = testService;
        _title = test.Title;
        Questions = new ObservableCollection<Question>(test.Questions);
    }
}


public class TeacherDashboardViewModel : ViewModelBase
{
    private readonly ITestService _testService;
    private readonly IQuestionService _questionService;
    private readonly ILoggingService _loggingService;
    private readonly IServiceProvider _serviceProvider;
    
    public event Action? LogoutRequested;

    public ObservableCollection<TestViewModel> Tests { get; } = new();
    public ObservableCollection<LogEntry> LogEntries { get; } = new();

    private TestViewModel? _selectedTest;
    public TestViewModel? SelectedTest
    {
        get => _selectedTest;
        set { Set(ref _selectedTest, value); OnPropertyChanged(nameof(Questions)); }
    }
    
    public ObservableCollection<Question>? Questions => SelectedTest?.Questions;
    
    private Question? _selectedQuestion;
    public Question? SelectedQuestion { get => _selectedQuestion; set => Set(ref _selectedQuestion, value); }

    public ICommand AddTestCommand { get; }
    public ICommand DeleteTestCommand { get; }
    public ICommand AssignTestCommand { get; }
    public ICommand AddQuestionCommand { get; }
    public ICommand EditQuestionCommand { get; }
    public ICommand DeleteQuestionCommand { get; }
    public ICommand RefreshLogsCommand { get; }
    public ICommand LogoutCommand { get; }

    public TeacherDashboardViewModel(ITestService testService, IQuestionService questionService, ILoggingService loggingService, IServiceProvider serviceProvider)
    {
        _testService = testService;
        _questionService = questionService;
        _loggingService = loggingService;
        _serviceProvider = serviceProvider;

        AddTestCommand = new RelayCommand(async _ => await AddTest());
        DeleteTestCommand = new RelayCommand(async _ => await DeleteTest(), _ => SelectedTest != null);
        AssignTestCommand = new RelayCommand(async _ => await AssignTest(), _ => SelectedTest != null);
        AddQuestionCommand = new RelayCommand(async _ => await AddQuestion(), _ => SelectedTest != null);
        EditQuestionCommand = new RelayCommand(async _ => await EditQuestion(), _ => SelectedTest != null && SelectedQuestion != null);
        DeleteQuestionCommand = new RelayCommand(async _ => await DeleteQuestion(), _ => SelectedTest != null && SelectedQuestion != null);
        RefreshLogsCommand = new RelayCommand(async _ => await LoadLogs());
        LogoutCommand = new RelayCommand(_ => LogoutRequested?.Invoke());

        _ = LoadTests();
        _ = LoadLogs();
    }
    
    private async Task LoadTests()
    {
        Tests.Clear();
        var testsFromDb = await _testService.GetAllTestsAsync();
        foreach (var t in testsFromDb)
        {
            Tests.Add(new TestViewModel(t, _testService));
        }
        SelectedTest = Tests.FirstOrDefault();
    }
    
    private async Task LoadLogs()
    {
        LogEntries.Clear();
        var logs = await _loggingService.GetLogsAsync();
        foreach (var log in logs)
        {
            LogEntries.Add(log);
        }
    }
    
    private async Task AddTest()
    {
        var newTest = await _testService.CreateTestAsync("Новий тест " + (Tests.Count + 1));
        var testVm = new TestViewModel(newTest, _testService);
        Tests.Add(testVm);
        SelectedTest = testVm;
    }
    
    private async Task DeleteTest()
    {
        if (SelectedTest == null) return;
        
        await _testService.DeleteTestAsync(SelectedTest.Id);
        Tests.Remove(SelectedTest);
        SelectedTest = Tests.FirstOrDefault();
    }
    
    private async Task AssignTest()
    {
        if (SelectedTest == null) return;

        var dbContext = _serviceProvider.GetRequiredService<AppDbContext>();
        var studentUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "student");

        if (studentUser != null)
        {
            await _testService.AssignTestAsync(SelectedTest.Id, studentUser.Id);
            MessageBox.Show($"Тест '{SelectedTest.Title}' призначено студенту 'student'.");
        }
        else
        {
            MessageBox.Show("Помилка: користувача 'student' не знайдено в базі.");
        }
    }
    
    private async Task AddQuestion()
    {
        if (SelectedTest == null) return;

        var editorVm = _serviceProvider.GetRequiredService<QuestionEditorViewModel>();
        var editorView = new QuestionEditorView(editorVm);
        
        if (editorView.ShowDialog() == true)
        {
            await _questionService.CreateQuestionAsync(SelectedTest.Id, editorVm.QuestionText, editorVm.AnswerOptions.Select(o => (o.Text, o.IsCorrect)).ToList());
            await LoadTests();
        }
    }

    private async Task EditQuestion()
    {
        if (SelectedQuestion == null) return;
        
        var editorVm = _serviceProvider.GetRequiredService<QuestionEditorViewModel>();
        editorVm.LoadQuestion(SelectedQuestion);
        var editorView = new QuestionEditorView(editorVm);

        if (editorView.ShowDialog() == true)
        {
            await _questionService.UpdateQuestionAsync(SelectedQuestion.Id, editorVm.QuestionText, editorVm.AnswerOptions.Select(o => (o.Text, o.IsCorrect)).ToList());
            await LoadTests();
        }
    }

    private async Task DeleteQuestion()
    {
        if (SelectedQuestion == null) return;
        
        await _questionService.DeleteQuestionAsync(SelectedQuestion.Id);
        await LoadTests();
    }
}