using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; // Make sure this is included for LINQ methods like .ToArray()
using System.Windows.Input; // For ICommand

namespace TestingAppWPF
{
    // Make sure BaseViewModel is defined and accessible within the TestingAppWPF namespace
    // It should contain the SetProperty method and implement INotifyPropertyChanged.
    public class QuizViewModel : BaseViewModel
    {
        private Questionnaire _quizModel;

        // Binds to TextBlock for question number display
        private string _currentQuestionNumberDisplay;
        public string CurrentQuestionNumberDisplay
        {
            get => _currentQuestionNumberDisplay;
            set => SetProperty(ref _currentQuestionNumberDisplay, value);
        }

        // Binds to ContentControl for question and answer content
        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                SetProperty(ref _currentQuestion, value);
                // Also update the Answers collection when CurrentQuestion changes
                // This ensures the ItemsControl/TextBox in XAML updates
                Answers = new ObservableCollection<Answer>(value.Answers);
                OnPropertyChanged(nameof(Answers)); // Notify UI that Answers collection has changed
            }
        }

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                SetProperty(ref _currentQuestionIndex, value);
                UpdateQuestionNavigationState(); // Update CanExecute for commands
                UpdateCurrentQuestionNumberDisplay(); // Update the display string
            }
        }

        // You had QuizTitle, but it's not currently used in the provided XAML,
        // unless you plan to add a TextBlock for it later.
        private string _quizTitle;
        public string QuizTitle
        {
            get => _quizTitle;
            set => SetProperty(ref _quizTitle, value);
        }

        // This ObservableCollection will hold the answers for the CURRENT question.
        // It's crucial because the XAML (ItemsControl) binds to this.
        private ObservableCollection<Answer> _answers;
        public ObservableCollection<Answer> Answers
        {
            get => _answers;
            set => SetProperty(ref _answers, value);
        }

        // Commands for navigation and submission
        public ICommand PreviousQuestionCommand { get; private set; }
        public ICommand NextQuestionCommand { get; private set; }
        public ICommand SubmitQuizCommand { get; private set; }

        // Properties to display results (optional, if you want to show results in the same window)
        private string _finalGradeTitle;
        public string FinalGradeTitle
        {
            get => _finalGradeTitle;
            set => SetProperty(ref _finalGradeTitle, value);
        }

        private int _totalScore;
        public int TotalScore
        {
            get => _totalScore;
            set => SetProperty(ref _totalScore, value);
        }

        // Constructor
        public QuizViewModel(Questionnaire quiz)
        {
            _quizModel = quiz;
            QuizTitle = quiz.Title ?? "Опрос"; // Use null-coalescing for title

            // Initialize commands before setting index to ensure CanExecute works
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion, CanExecutePreviousQuestion);
            NextQuestionCommand = new RelayCommand(NextQuestion, CanExecuteNextQuestion);
            SubmitQuizCommand = new RelayCommand(SubmitQuiz, CanExecuteSubmitQuiz); // Submit only when on last question

            CurrentQuestionIndex = 0; // Setting index will trigger LoadCurrentQuestion indirectly
            LoadCurrentQuestion(); // Manually load the first question
        }

        // Method to load the question based on CurrentQuestionIndex
        private void LoadCurrentQuestion()
        {
            if (_quizModel.Questions != null && _quizModel.Questions.Length > CurrentQuestionIndex && CurrentQuestionIndex >= 0)
            {
                // This assignment triggers the setter of CurrentQuestion, which also updates Answers
                CurrentQuestion = _quizModel.Questions[CurrentQuestionIndex];
            }
            UpdateQuestionNavigationState(); // Update command states
            UpdateCurrentQuestionNumberDisplay(); // Update display string
        }

        // Update the display for "Question X of Y"
        private void UpdateCurrentQuestionNumberDisplay()
        {
            int totalQuestions = _quizModel.Questions?.Length ?? 0;
            CurrentQuestionNumberDisplay = $"Вопрос {CurrentQuestionIndex + 1} из {totalQuestions}";
        }

        // Updates CanExecute state for navigation commands
        private void UpdateQuestionNavigationState()
        {
            // Call InvalidateRequerySuggested to re-evaluate CanExecute for RelayCommands
            ((RelayCommand)PreviousQuestionCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextQuestionCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SubmitQuizCommand).RaiseCanExecuteChanged();
        }

        // --- Command Implementations ---

        private void PreviousQuestion(object parameter)
        {
            // SaveUserAnswers(); // Changes are automatically reflected via TwoWay Binding
            CurrentQuestionIndex--;
            LoadCurrentQuestion();
        }

        private bool CanExecutePreviousQuestion(object parameter)
        {
            return CurrentQuestionIndex > 0;
        }

        private void NextQuestion(object parameter)
        {
            // SaveUserAnswers(); // Changes are automatically reflected via TwoWay Binding
            CurrentQuestionIndex++;
            LoadCurrentQuestion();
        }

        private bool CanExecuteNextQuestion(object parameter)
        {
            return _quizModel.Questions != null && CurrentQuestionIndex < _quizModel.Questions.Length - 1;
        }

        private void SubmitQuiz(object parameter)
        {
            // SaveUserAnswers(); // Changes are automatically reflected via TwoWay Binding

            QuizResults results = _quizModel.GradeUserAnswers();
            FinalGradeTitle = results.FinalGrade?.Title ?? "Не оценено";
            TotalScore = results.TotalScore;

            // Here you might typically open a new window to show results,
            // or navigate to a results view/state within the same window.
            // For simplicity, we're just updating properties in this ViewModel.
            System.Diagnostics.Debug.WriteLine($"Quiz Submitted! Final Grade: {FinalGradeTitle}, Score: {TotalScore}");
            System.Diagnostics.Debug.WriteLine($"Correct: {results.CorrectAnswersCount}, Incorrect: {results.IncorrectAnswersCount}, Penalty: {results.TotalPenalty}");

            // Disable further navigation after submission
            ((RelayCommand)PreviousQuestionCommand).RaiseCanExecuteChanged();
            ((RelayCommand)NextQuestionCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SubmitQuizCommand).RaiseCanExecuteChanged();
        }

        private bool CanExecuteSubmitQuiz(object parameter)
        {
            // Allow submission only on the last question
            return _quizModel.Questions != null && CurrentQuestionIndex == _quizModel.Questions.Length - 1;
        }

        // The SaveUserAnswers() method is no longer strictly necessary
        // for basic checkbox/radio button input if IsUserSelected in Answer
        // is properly bound with Mode=TwoWay and Answer.IsUserSelected uses SetProperty.
        // For TextBox, the content is updated directly in Answers[0].Content
        // due to UpdateSourceTrigger=PropertyChanged.
        // It could be used for validation logic if needed before moving to next question.
    }

    // --- RelayCommand Class (You might already have this from BaseViewModel setup) ---
    // If not, add it in a separate file like RelayCommand.cs or here if you prefer.
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}