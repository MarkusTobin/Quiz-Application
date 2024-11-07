using Labb3___GUI.Command;
using Labb3___GUI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

namespace Labb3___GUI.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        public string QuestionOfTotalQuestion => $"Question {CurrentQuestionNumber} of {TotalQuestions}";
        private string[] _currentAnswers;
        private readonly Random random = new Random();
        private QuestionPackViewModel ActivePack;
        public void StartNewQuiz(List<Question> questions)
        {
            ShuffledQuestions = questions.OrderBy(q => Guid.NewGuid()).ToList();

            if (ShuffledQuestions.Any())
            {
                TotalQuestions = ShuffledQuestions.Count;
                CurrentQuestionNumber = 1;
                CurrentQuestion = ShuffledQuestions.FirstOrDefault();
                CorrectAnswerCount = 0;
                TimeRemaining = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
                RaisePropertyChanged(nameof(TimeRemaining));
                RaisePropertyChanged(nameof(QuestionOfTotalQuestion));
                timer.Start();
            }
            else
            {
                IsQuizFinished = true;
                EndOfQuizMessage = "No questions available.";
            }
        }
        private readonly MainWindowViewModel? mainWindowViewModel;
        private DispatcherTimer timer;


        private Question _currentQuestion;
        public DelegateCommand AnswerCommand { get; }

        public DelegateCommand ResetQuizCommand { get; }
        public DelegateCommand StartTimerCommand { get; }

        private ObservableCollection<string> _currentAnswerOptions;
        public ObservableCollection<string> CurrentAnswerOptions
        {
            get => _currentAnswerOptions;
            private set
            {
                _currentAnswerOptions = value;
                RaisePropertyChanged(nameof(CurrentAnswerOptions));
            }
        }
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                if (_currentQuestion != value)
                {
                    _currentQuestion = value;
                    RaisePropertyChanged(nameof(CurrentQuestion));

                    if (_currentQuestion != null)
                    {
                        _currentAnswers = GetShuffledAnswers(CurrentQuestion);
                    }
                    else
                    {
                        _currentAnswers = Array.Empty<string>();
                    }

                    RaisePropertyChanged(nameof(Answer1));
                    RaisePropertyChanged(nameof(Answer2));
                    RaisePropertyChanged(nameof(Answer3));
                    RaisePropertyChanged(nameof(Answer4));
                }
            }
        }

        private string[] GetShuffledAnswers(Question question)
        {
            var answers = new List<string>
        {
        question.IncorrectAnswer1,
        question.IncorrectAnswer2,
        question.IncorrectAnswer3,
        question.CorrectAnswer 
        };

            return answers.OrderBy(a => random.Next()).ToArray();
        }

        public string Answer1 => _currentAnswers?.Length > 0 ? _currentAnswers[0] : string.Empty;
        public string Answer2 => _currentAnswers?.Length > 1 ? _currentAnswers[1] : string.Empty;
        public string Answer3 => _currentAnswers?.Length > 2 ? _currentAnswers[2] : string.Empty;
        public string Answer4 => _currentAnswers?.Length > 3 ? _currentAnswers[3] : string.Empty;

        private int _currentQuestionNumber;
        public int CurrentQuestionNumber
        {
            get => _currentQuestionNumber; set
            {
                _currentQuestionNumber = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(QuestionOfTotalQuestion));
            }
        }

        private int _totalQuestions;
        public int TotalQuestions
        {
            get => _totalQuestions;
            set
            {
                _totalQuestions = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(QuestionOfTotalQuestion));
            }
        }

        private List<Question> _shuffledQuestions;
        public List<Question> ShuffledQuestions
        {
            get => _shuffledQuestions;
            set
            {
                _shuffledQuestions = value;
                TotalQuestions = _shuffledQuestions?.Count ?? 0;
                RaisePropertyChanged(nameof(ShuffledQuestions));
                RaisePropertyChanged(nameof(TotalQuestions));

            }
        }
        private int _timeRemaining;
        public int TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (_timeRemaining != value)
                {
                    _timeRemaining = value;
                    RaisePropertyChanged(nameof(TimeRemaining));
                }
            }
        }
        private string _endOfQuizMessage;
        public string EndOfQuizMessage                         
        {                                                      
            get => _endOfQuizMessage;                          
            set                                                
            {                                                  
                _endOfQuizMessage = value;                     
                RaisePropertyChanged(nameof(EndOfQuizMessage));
            }
        }
        private bool _isQuizFinished;
        public bool IsQuizFinished
        {
            get => _isQuizFinished;
            set
            {
                if (_isQuizFinished != value)
                {
                    _isQuizFinished = value;
                    IsQuizRunning = !value;
                    RaisePropertyChanged(nameof(IsQuizFinished));
                    IsQuizRunning = !value;
                }
            }
        }
        private bool _isStartButtonVisible = true;
        public bool IsStartButtonVisible
        {
            get => _isStartButtonVisible;
            set
            {
                _isStartButtonVisible = value;
                RaisePropertyChanged(nameof(IsStartButtonVisible));
            }
        }

        private bool _isQuizRunning;
        public bool IsQuizRunning
        {
            get => _isQuizRunning;
            set
            {
                _isQuizRunning = value;
                RaisePropertyChanged(nameof(IsQuizRunning));
            }
        }

        private void StartTimer(object? parameter)
        {

            TimeRemaining = mainWindowViewModel.ActivePack.TimeLimitInSeconds;

            IsStartButtonVisible = false;
            IsQuizRunning = true;
            RaisePropertyChanged(nameof(TimeRemaining));
            timer.Start();
        }

        private void ResetQuiz(object? parameter)
        {
            ResetButtonColors();
            CurrentQuestionNumber = 1;
            CorrectAnswerCount = 0;
            IsQuizFinished = false;
            IsQuizRunning = true;
            IsStartButtonVisible = false;
            TimeRemaining = mainWindowViewModel.ActivePack.TimeLimitInSeconds;

            ShuffledQuestions = ShuffledQuestions.OrderBy(q => Guid.NewGuid()).ToList();
            CurrentQuestion = ShuffledQuestions.FirstOrDefault();


            RaisePropertyChanged(nameof(ShuffledQuestions));
            timer.Start();
        }

        private int _correctAnswerCount;
        public int CorrectAnswerCount
        {
            get => _correctAnswerCount;
            set
            {
                _correctAnswerCount = value;
                RaisePropertyChanged(nameof(CorrectAnswerCount));
                RaisePropertyChanged(nameof(TotalScore));
            }
        }

        public DelegateCommand UpdateButtonCommand { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {

            CurrentQuestion = new Question();
            this.mainWindowViewModel = mainWindowViewModel;

            ShuffledQuestions = mainWindowViewModel.ActivePack.Questions.OrderBy(q => Guid.NewGuid()).ToList();
            TotalQuestions = ShuffledQuestions.Count;

            CurrentQuestionNumber = 1;
            CurrentQuestion = ShuffledQuestions.FirstOrDefault();
            CorrectAnswerCount = 0;

            TimeRemaining = mainWindowViewModel.ActivePack?.TimeLimitInSeconds ?? 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;

            AnswerCommand = new DelegateCommand(AnswerSelected);
            StartTimerCommand = new DelegateCommand(StartTimer);
            ResetQuizCommand = new DelegateCommand(ResetQuiz);
        }

        public string TotalScore => $"{CorrectAnswerCount} of {TotalQuestions} correct!";
        private async void AnswerSelected(object? parameter)
        {
            string selectedAnswer = parameter as string;
            if (CurrentQuestion != null && selectedAnswer != null)
            {

                ResetButtonColors();


                if (CurrentQuestion.CorrectAnswer == selectedAnswer)
                {
                    CorrectAnswerCount++;
                    SetButtonColor(selectedAnswer, Brushes.Green);
                }
                else
                {
                    SetButtonColor(selectedAnswer, Brushes.Red);
                    SetButtonColor(CurrentQuestion.CorrectAnswer, Brushes.Green);
                }

                await Task.Delay(1000);
                await NextQuestion();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Debug.WriteLine($"Time Remaining: {TimeRemaining}");
            if (TimeRemaining > 0)
            {
                TimeRemaining--;
            }
            else
            {
                ShowCorrectAnswer();
                Task.Delay(1000).ContinueWith(t => NextQuestion());
            }
        }
        public async Task NextQuestion()
        {
            if (CurrentQuestionNumber < TotalQuestions)
            {
                ResetButtonColors();
                CurrentQuestionNumber++;
                CurrentQuestion = ShuffledQuestions[CurrentQuestionNumber - 1];

                TimeRemaining = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
                RaisePropertyChanged(nameof(TimeRemaining));
                timer.Start();
            }
            else
            {
                timer.Stop();
                IsQuizFinished = true;
                RaisePropertyChanged(TotalScore);
            }
        }

        private async Task ShowCorrectAnswer()
        {
            if (AnswerSelected == ShowCorrectAnswer)
            {
                SetButtonColor(CurrentQuestion.CorrectAnswer, Brushes.Green);
                CorrectAnswerCount++;
            }
            else
            {
                SetButtonColor(CurrentQuestion.CorrectAnswer, Brushes.Green);
                return;
            }
        }
        private void ResetButtonColors()
        {
            Option1StatusColor = Brushes.White;
            Option2StatusColor = Brushes.White;
            Option3StatusColor = Brushes.White;
            Option4StatusColor = Brushes.White;
        }

        private void SetButtonColor(string answer, SolidColorBrush color)
        {
            if (answer == _currentAnswers[0]) Option1StatusColor = color;  
            else if (answer == _currentAnswers[1]) Option2StatusColor = color;
            else if (answer == _currentAnswers[2]) Option3StatusColor = color; 
            else if (answer == _currentAnswers[3]) Option4StatusColor = color; 
        }

        private SolidColorBrush _option1StatusColor;
        public SolidColorBrush Option1StatusColor
        {
            get => _option1StatusColor;
            set
            {
                _option1StatusColor = value;
                RaisePropertyChanged(nameof(Option1StatusColor));
            }
        }

        private SolidColorBrush _option2StatusColor;
        public SolidColorBrush Option2StatusColor
        {
            get => _option2StatusColor;
            set
            {
                _option2StatusColor = value;
                RaisePropertyChanged(nameof(Option2StatusColor));
            }
        }

        private SolidColorBrush _option3StatusColor;
        public SolidColorBrush Option3StatusColor
        {
            get => _option3StatusColor;
            set
            {
                _option3StatusColor = value;
                RaisePropertyChanged(nameof(Option3StatusColor));
            }
        }

        private SolidColorBrush _option4StatusColor;
        public SolidColorBrush Option4StatusColor
        {
            get => _option4StatusColor;
            set
            {
                _option4StatusColor = value;
                RaisePropertyChanged(nameof(Option4StatusColor));
            }
        }

    }
}