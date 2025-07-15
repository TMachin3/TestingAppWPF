using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestingAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Question q2 = new Question
            {
                QuestionType = questionType.checkBox,
                Content = "Which of these are programming languages? (Select all that apply)",
                Answers = new Answer[]
                {
                    new Answer { Content = "Python", IsTrue = true, Score = 1 },
                    new Answer { Content = "HTML", IsTrue = false, Score = 0 },
                    new Answer { Content = "Java", IsTrue = true, Score = 1 },
                    new Answer { Content = "CSS", IsTrue = false, Score = 0 }
                }
            };
            foreach (var answer in q2.Answers)
            {
                answer.IsUserSelected = true;
            }
            Debug.WriteLine(q2.IsCorrect);
            Console.WriteLine(q2.IsCorrect);
            InitializeComponent();
        }
    }

}