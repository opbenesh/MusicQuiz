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
using System.Threading;
using System.Diagnostics;

namespace MusicQuiz.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QuizDB QuizDB;
        private MusicPlayer _player;
        private QuestionVM _currentQuestion;

        private const int QUESTION_VALUE = 10;
        private int _score = 0;

        private BackgroundWorker getQuestionWorker = new BackgroundWorker();
        

        public MainWindow()
        {
            _player = new MusicPlayer();
            QuizDB = new QuizDB();
            getQuestionWorker.DoWork += new DoWorkEventHandler(getQuestionWorker_DoWork);
            getQuestionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(getQuestionWorker_RunWorkerCompleted);
            this.DataContext = _currentQuestion;
            InitializeComponent();
            ScoreTB.DataContext = _score;
        }

        void getQuestionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_currentQuestion != null) Thread.Sleep(700);
            Question question=null;
            while(question==null)
                try
                {
                    question = QuizDB.CreateNewQuestion();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            this.Dispatcher.Invoke(new Action(()=>this._currentQuestion = new QuestionVM(question)));
        }

        void getQuestionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _player.Play(_currentQuestion.File);
            this.DataContext = _currentQuestion;
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
                ScoreTB.DataContext = _score;
            }
            else
            {
                _currentQuestion.Options.Single(o => o.Option.IsCorrect).IsUserCorrect = true;
            }
            _currentQuestion.IsActive = false;
            _player.Stop();
            ChangeQuestion();
        }

        private void ChangeQuestion()
        {
            getQuestionWorker.RunWorkerAsync();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getQuestionWorker.RunWorkerAsync();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _player.Dispose();
        }
    }

    public class Question
    {
        public string Title { get; set; }

        public List<Option> Options { get; set; }

        public MusicFile File { get; set; }
    }

    class QuestionVM
    {
        public QuestionVM(Question question)
        {
            this.IsActive = true;
            this.Question = question;
            this.Options = question.Options.Select(o => new OptionVM(o)).ToList();
            this.File = question.File;
        }

        public MusicFile File { get; set; }

        public List<OptionVM> Options { get; set; }

        public Question Question { get; set; }

        public bool IsActive { get; set; }
    }

    public class Option
    {
        public TagLib.File File { get; set; }

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
