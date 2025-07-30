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
            var quizData = new Questionnaire
            {
                Title = "Тест по WPF и C#",
                Subject = "Программирование",
                Questions = new Question[]
                {
                // Вопрос 1: RadioButton
                new Question
                {
                    Content = "Какая библиотека Python чаще всего используется для создания статических и анимированных графиков, включая визуализацию графов?",
                    QuestionType = questionType.radioButton,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "Pandas", Correct = false, Score = 0 },
                        new Answer { Content = "NumPy", Correct = false, Score = 0 },
                        new Answer { Content = "Matplotlib", Correct = true, Score = 1 },
                        new Answer { Content = "Scikit-learn", Correct = false, Score = 0 }
                    }
                },

                // Вопрос 2: CheckBox
                new Question
                {
                    Content = "Какие элементы графа можно динамически изменять (анимировать) при визуализации его состояния?",
                    QuestionType = questionType.checkBox,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "Цвет вершин", Correct = true, Score = 1 },
                        new Answer { Content = "Положение вершин", Correct = true, Score = 1 },
                        new Answer { Content = "Толщина рёбер", Correct = true, Score = 1 },
                        new Answer { Content = "Текст меток рёбер", Correct = true, Score = 1 }
                    }
                },

                // Вопрос 3: TextBox
                new Question
                {
                    Content = "Как называется функция в Matplotlib, которая является ключевой для создания анимаций, многократно вызывая другую функцию для обновления графика?",
                    QuestionType = questionType.textBox,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "FuncAnimation", Correct = true, Score = 1 }
                    }
                },

                // Вопрос 4: RadioButton
                new Question
                {
                    Content = "Для чего обычно используется библиотека NetworkX при создании анимированных изображений графов?",
                    QuestionType = questionType.radioButton,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "Для непосредственного создания GIF-файлов", Correct = false, Score = 0 },
                        new Answer { Content = "Для управления данными графа (вершины, рёбра) и выполнения алгоритмов", Correct = true, Score = 1 },
                        new Answer { Content = "Для высокопроизводительных расчётов на GPU", Correct = false, Score = 0 },
                        new Answer { Content = "Для создания 3D-моделей графов", Correct = false, Score = 0 }
                    }
                },

                // Вопрос 5: CheckBox
                new Question
                {
                    Content = "Что нужно импортировать из модуля `matplotlib.animation` для работы с анимацией?",
                    QuestionType = questionType.checkBox,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "AnimationPlayer", Correct = false, Score = 0 },
                        new Answer { Content = "FuncAnimation", Correct = true, Score = 1 },
                        new Answer { Content = "MovieWriter", Correct = true, Score = 1 }, // Для сохранения в видео
                        new Answer { Content = "Animator", Correct = false, Score = 0 }
                    }
                },
                
                // Вопрос 6: TextBox
                new Question
                {
                    Content = "В каком формате чаще всего сохраняются анимации, созданные с помощью Matplotlib, для воспроизведения в веб-браузерах или презентациях?",
                    QuestionType = questionType.textBox,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "GIF", Correct = true, Score = 1 } // Или "MP4"
                    }
                },

                // Вопрос 7: RadioButton
                new Question
                {
                    Content = "Какое из следующих утверждений НЕВЕРНО об анимации графов?",
                    QuestionType = questionType.radioButton,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "Она помогает понять динамику алгоритмов обхода.", Correct = false, Score = 0 },
                        new Answer { Content = "Она всегда требует отдельного процесса рендеринга видео.", Correct = true, Score = 1 }, // Не всегда, можно в Jupyter отображать
                        new Answer { Content = "Для сложных графов может потребоваться значительное время для рендеринга.", Correct = false, Score = 0 },
                        new Answer { Content = "Визуализация весов рёбер может быть частью анимации.", Correct = false, Score = 0 }
                    }
                },

                // Вопрос 8: CheckBox
                new Question
                {
                    Content = "Какие параметры обычно передаются в функцию `FuncAnimation`?",
                    QuestionType = questionType.checkBox,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "Объект Figure (фигура Matplotlib)", Correct = true, Score = 1 },
                        new Answer { Content = "Функция обновления (update function)", Correct = true, Score = 1 },
                        new Answer { Content = "Количество кадров (frames)", Correct = true, Score = 1 },
                        new Answer { Content = "Интервал между кадрами (interval)", Correct = true, Score = 1 }
                    }
                },
                
                // Вопрос 9: TextBox
                new Question
                {
                    Content = "Как называется процесс, при котором каждый кадр анимации графа последовательно строится и отображается?",
                    QuestionType = questionType.textBox,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "Рендеринг", Correct = true, Score = 1 }
                    }
                },
                
                // Вопрос 10: RadioButton
                new Question
                {
                    Content = "Что необходимо установить в вашей системе, помимо Python и Matplotlib, чтобы сохранять анимации в формате MP4?",
                    QuestionType = questionType.radioButton,
                    Answers = new Answer[]
                    {
                        new Answer { Content = "OpenCV", Correct = false, Score = 0 },
                        new Answer { Content = "Pillow", Correct = false, Score = 0 },
                        new Answer { Content = "FFmpeg", Correct = true, Score = 1 },
                        new Answer { Content = "Pygame", Correct = false, Score = 0 }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }
}
