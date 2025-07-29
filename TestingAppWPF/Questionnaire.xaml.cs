using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestingAppWPF;

namespace TestingAppWPF
{
   
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>v
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            // Создаем экземпляр Questionnaire (ваша модель данных для квиза)
            // В реальном приложении это может быть загружено из файла, БД и т.д.
            // Для примера создадим тестовые данные здесь.
            var quizData = new Questionnaire
            {
                Title = "Тест по WPF и C#",
                Subject = "Программирование",
                Questions = new Question[]
                {
                    new Question
                    {
                        Content = "Какой фреймворк используется для создания настольных приложений на C#?",
                        QuestionType =  questionType.radioButton,
                        Answers = new  Answer[]
                        {
                            new  Answer { Content = "ASP.NET", Score = -1, Correct = false },
                            new  Answer { Content = "WinForms", Score = 5, Correct = false },
                            new Answer { Content = "WPF", Score = 10, Correct = true },
                            new Answer { Content = "Xamarin", Score = -1, Correct = false }
                        }
                    },
                    new Question
                    {
                        Content = "Выберите правильные утверждения о C# (несколько вариантов):",
                        QuestionType = questionType.checkBox,
                        Answers = new Answer[]
                        {
                            new Answer { Content = "C# — это объектно-ориентированный язык.", Score = 5, Correct = true },
                            new Answer { Content = "C# компилируется в байт-код.", Score = 5, Correct = true },
                            new Answer { Content = "C# в основном используется для веб-разработки.", Score = -5, Correct = false },
                            new Answer { Content = "C# поддерживается только на Windows.", Score = -5, Correct = false }
                        }
                    },
                    new Question
                    {
                        Content = "Назовите столицу Франции:",
                        QuestionType = questionType.textBox,
                        Answers = new Answer[]
                        {
                            new Answer { Content = "Париж", Score = 10, Correct = true }
                        }
                    }
                }
            };
            quizData.SetDefaultPercentageGrades(); // Инициализируем оценки

            // Создаем ViewModel для этого окна (QuizViewModel)
            var quizViewModel = new QuizViewModel(quizData);

            // Устанавливаем DataContext окна на нашу ViewModel
            this.DataContext = quizViewModel;
        }

    }
}
