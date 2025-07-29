using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestingAppWPF
{
    public class QuizViewModel : BaseViewModel
    {
        private Questionnaire _quizModel;

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set => SetProperty(ref _currentQuestion, value); // Используем SetProperty
        }

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set => SetProperty(ref _currentQuestionIndex, value); // Используем SetProperty
        }

        private string _quizTitle;
        public string QuizTitle
        {
            get => _quizTitle;
            set => SetProperty(ref _quizTitle, value); // Используем SetProperty
        }

        // ObservableCollection<Answer> напрямую, если Answer тоже реализует INotifyPropertyChanged
        // или если IsUserSelected достаточно менять напрямую без отдельного AnswerViewModel
        public ObservableCollection<Answer> Answers { get; set; }


        public ICommand NextQuestionCommand { get; private set; }
        public ICommand SubmitQuizCommand { get; private set; }

        public QuizViewModel(Questionnaire quiz)
        {
            _quizModel = quiz;
            QuizTitle = quiz.Title;
            CurrentQuestionIndex = 0;

            LoadCurrentQuestion();

            // Инициализируем команды
            NextQuestionCommand = new RelayCommand(NextQuestion, CanExecuteNextQuestion);
            SubmitQuizCommand = new RelayCommand(SubmitQuiz);
        }

        private void LoadCurrentQuestion()
        {
            if (_quizModel.Questions != null && _quizModel.Questions.Length > CurrentQuestionIndex)
            {
                CurrentQuestion = _quizModel.Questions[CurrentQuestionIndex];
                // Создаем новую ObservableCollection для Answers, чтобы UI мог отслеживать изменения списка
                Answers = new ObservableCollection<Answer>(CurrentQuestion.Answers);

                // Если Answer не реализует INotifyPropertyChanged, но IsUserSelected меняется
                // и UI должен реагировать, вам нужно обернуть Answer в AnswerViewModel,
                // которая будет реализовывать INotifyPropertyChanged для IsSelected.
                // В вашем текущем коде Answer.IsUserSelected является обычным свойством.
                // Если UI должен обновляться при изменении IsUserSelected,
                // вам нужно либо сделать Answer INotifyPropertyChanged, либо использовать AnswerViewModel.
            }
        }

        private void NextQuestion(object parameter) // Параметр для ICommand
        {
            SaveUserAnswers();

            CurrentQuestionIndex++;
            LoadCurrentQuestion(); // Это вызовет обновление CurrentQuestion и Answers
        }

        private bool CanExecuteNextQuestion(object parameter) // Параметр для ICommand
        {
            return _quizModel.Questions != null && CurrentQuestionIndex < _quizModel.Questions.Length - 1;
        }

        private void SubmitQuiz(object parameter) // Параметр для ICommand
        {
            SaveUserAnswers();

            QuizResults results = _quizModel.GradeUserAnswers();
            // Здесь вы можете обработать результаты, например, отобразить их в новом окне
            // или обновить свойства в QuizViewModel для отображения результатов в текущем View.
            // Например:
            // FinalScore = results.TotalScore;
            // FinalGradeTitle = results.FinalGrade?.Title;
            // OnPropertyChanged(nameof(FinalScore));
            // OnPropertyChanged(nameof(FinalGradeTitle));
        }

        private void SaveUserAnswers()
        {
            // Поскольку Answers теперь ObservableCollection<Answer>,
            // а Answer.IsUserSelected - это свойство, привязанное к CheckBox/RadioButton,
            // изменения в Answer.IsUserSelected будут происходить напрямую,
            // если Answer.IsUserSelected реализует INotifyPropertyChanged,
            // или если привязка Mode=TwoWay.
            // Если Answer не реализует INotifyPropertyChanged,
            // и вы используете CheckBox (который напрямую меняет IsUserSelected),
            // то явного "сохранения" здесь не потребуется, так как изменения уже в модели.
            // Если вы используете AnswerViewModel (как в предыдущем примере),
            // то здесь нужно будет синхронизировать IsSelected из AnswerViewModel с IsUserSelected в Model.Answer.
            // Для простоты, если IsUserSelected в Model.Answer прямо привязывается, то этот метод может быть пустым
            // или использоваться для дополнительной логики валидации/агрегации перед переходом к следующему вопросу.
        }
    }

}
