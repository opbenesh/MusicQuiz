using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace MusicQuiz.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QuestionVM _currentQuestion;

        private const int QUESTION_VALUE = 10;
        private int _score = 0;

        public MainWindow()
        {
            var q = new Question()
            {
                Title = "Which artist?",
                Options = new List<Option>()
                {
                    new Option(){Title="Pink Floyd", IsCorrect=false},
                    new Option(){Title="The Beatles", IsCorrect=false},
                    new Option(){Title="Nirvana", IsCorrect=true},
                    new Option(){Title="Led Zeppelin", IsCorrect=false}
                }
            };
            this.DataContext = _currentQuestion = new QuestionVM(q);
            InitializeComponent();
        }


        private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void OptionsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_currentQuestion.IsActive || e.AddedItems.Count == 0) return;
            var option = e.AddedItems.Cast<OptionVM>().Single() as OptionVM;
            if (option == null)
                return;
            if ((option.IsUserCorrect = option.Option.IsCorrect).Value)
            {
                _score += QUESTION_VALUE;
            }
            _currentQuestion.IsActive = false;
            //ChangeQuestion();
        }

        private void ChangeQuestion()
        {
            throw new NotImplementedException();
        }
    }

    public class Question
    {
        public string Title { get; set; }

        public List<Option> Options { get; set; }
    }

    class QuestionVM
    {
        public QuestionVM(Question question)
        {
            this.IsActive = true;
            this.Options = question.Options.Select(o => new OptionVM(o)).ToList();
        }

        public List<OptionVM> Options { get; set; }

        public bool IsActive { get; set; }
    }

    public class Option
    {
        public string Title { get; set; }

        public bool IsCorrect { get; set; }
    }

    class OptionVM:INotifyPropertyChanged
    {
        public OptionVM(Option option)
        {
            this.Option = option;
        }

        private Option _option;
        public Option Option
        {
            get
            {
                return _option;
            }
            set
            {
                _option = value;
                NotifyPropertyChanged("Option");
            }
        }

        private bool? _isUserCorrect;
        public bool? IsUserCorrect
        {
            get
            {
                return _isUserCorrect;
            }
            set
            {
                _isUserCorrect = value;
                NotifyPropertyChanged("IsUserCorrect");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
