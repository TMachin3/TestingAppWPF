using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestingAppWPF
{
    public class QuestionViewModel : INotifyPropertyChanged
    {
        private Question? _currentQuestion;
        public event PropertyChangedEventHandler PropertyChanged;
        public Question CurrentQuestionnaire 
        { 
            get => _currentQuestion; 
            set
            {
                if (_currentQuestion != value)
                {
                    _currentQuestion = value;
                    OnPropertyChanged();
                }
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Console.Beep();
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
        }
    }
}
