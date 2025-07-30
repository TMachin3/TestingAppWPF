using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            string message = $"Тест завершен!\n\n" +
                             $"Набранные баллы: {results.TotalScore}\n" +
                             $"Правильных ответов: {results.CorrectAnswersCount}\n" +
                             $"Неправильных ответов: {results.IncorrectAnswersCount}\n" +
                             $"Штрафные баллы: {results.TotalPenalty}\n\n" +
                             $"Итоговая оценка: {results.FinalGrade?.Title ?? "Не оценено"}";

            // Display the message box
            MessageBox.Show(message, "Результаты теста", MessageBoxButton.OK, MessageBoxImage.Information);

            // Optionally, disable further navigation or perform other actions after submission
            // For example, you might want to prevent further question changes:
            // CurrentQuestionIndex = _quizModel.Questions.Length; // Move to a "results" state, if you add one
            
            //((RelayCommand)PreviousQuestionCommand).RaiseCanExecuteChanged();
            //((RelayCommand)NextQuestionCommand).RaiseCanExecuteChanged();
            //((RelayCommand)SubmitQuizCommand).RaiseCanExecuteChanged(); // Submit button will now be disabled
        }

        private void SaveUserAnswers()
        {

        }
    }

}
