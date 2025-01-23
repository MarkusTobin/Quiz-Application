using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;
using Labb3___GUI.Command;
using Labb3___GUI.Model;
using Labb3___GUI.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Labb3___GUI.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly Random _random = new Random();
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private readonly MongoDBService _mongoDBService;
        private DispatcherTimer _questionTimer;
        private Question _currentQuestion;
        private string[] _currentAnswers;
        private TimeSpan _sessionTimer;
        private string _playerName;
        private ObjectId _questionPackId;
        private string _questionPackName;
        private List<PlayerAnswer> _answers;
        private ObservableCollection<PlayerResult> _top5PlayerResult;
        private int _currentQuestionNumber;
        private int _totalQuestions;
        private List<Question> _shuffledQuestions;
        private int _timeRemaining;
        private string _endOfQuizMessage;
        private bool _isQuizFinished;
        private bool _isStartButtonVisible = true;
        private bool _isQuizRunning;
        private int _correctAnswerCount;
        private bool _canAnswer = true;
        private SolidColorBrush _option1StatusColor;
        private SolidColorBrush _option2StatusColor;
        private SolidColorBrush _option3StatusColor;
        private SolidColorBrush _option4StatusColor;
        private double _option1Percentage;
        private double _option2Percentage;
        private double _option3Percentage;
        private double _option4Percentage;
        private bool _showPercentages;


        public DelegateCommand AnswerCommand { get; }
        public DelegateCommand ResetQuizCommand { get; }
        public DelegateCommand StartTimerCommand { get; }
        public DelegateCommand UpdateButtonCommand { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel, MongoDBService mongoDBService)
        {
            Answers = new List<PlayerAnswer>();
            CurrentQuestion = new Question();
            _mainWindowViewModel = mainWindowViewModel;
            _mongoDBService = mongoDBService;

            ShuffledQuestions = mainWindowViewModel.ActivePack.Questions.OrderBy(q => Guid.NewGuid()).ToList();
            TotalQuestions = ShuffledQuestions.Count;

            CurrentQuestionNumber = 1;
            CurrentQuestion = ShuffledQuestions.FirstOrDefault();
            CorrectAnswerCount = 0;

            TimeRemaining = mainWindowViewModel.ActivePack?.TimeLimitInSeconds ?? 0;
            _questionTimer = new DispatcherTimer();
            _questionTimer.Interval = TimeSpan.FromSeconds(1);
            _questionTimer.Tick += TimerTick;

            AnswerCommand = new DelegateCommand(AnswerSelected);
            StartTimerCommand = new DelegateCommand(StartTimer);
            ResetQuizCommand = new DelegateCommand(ResetQuiz);
        }

        public async void StartNewQuiz(List<Question> questions)
        {
            Answers.Clear();
            QuestionPackId = _mainWindowViewModel.ActivePack.QuestionPack.Id;
            QuestionPackName = _mainWindowViewModel.ActivePack.Name;
            ShuffledQuestions = questions.OrderBy(q => Guid.NewGuid()).ToList();

            if (ShuffledQuestions.Any())
            {
                TotalQuestions = ShuffledQuestions.Count;
                CurrentQuestionNumber = 1;
                CurrentQuestion = ShuffledQuestions.FirstOrDefault();
                CorrectAnswerCount = 0;
                TimeRemaining = _mainWindowViewModel.ActivePack.TimeLimitInSeconds;
                RaisePropertyChanged(nameof(TimeRemaining));
                RaisePropertyChanged(nameof(QuestionOfTotalQuestion));
                _questionTimer.Start();
                SessionTimer = TimeSpan.Zero;
            }
            else
            {
                IsQuizFinished = true;
                EndOfQuizMessage = "No questions available.";
            }
        }

        private async Task EndQuizPlayerResult()
        {
            var playerResult = new PlayerResult
            {
                PlayerName = _playerName,
                QuestionPackId = _questionPackId,
                QuestionPackName = _questionPackName,
                Answers = _answers,
                TotalCorrectAnswers = _correctAnswerCount,
                TotalQuestions = _totalQuestions,
                TotalTime = _sessionTimer,
                DatePlayed = DateTime.UtcNow
            };
            await _mongoDBService.SavePlayerResult(playerResult);
            var leaderBoard = await _mongoDBService.GetTopPlayerResults(_questionPackId);
            Top5PlayerResult = new ObservableCollection<PlayerResult>(leaderBoard);
            Debug.WriteLine("Leaderboard updated");
        }

        private void StartTimer(object? parameter)
        {
            TimeRemaining = _mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            IsStartButtonVisible = false;
            IsQuizRunning = true;
            RaisePropertyChanged(nameof(TimeRemaining));
            _questionTimer.Start();
        }

        private void ResetQuiz(object? parameter)
        {
            ResetButtonColors();
            IsQuizFinished = false;
            IsQuizRunning = false;
            IsStartButtonVisible = true;
            ShuffledQuestions = ShuffledQuestions.OrderBy(q => Guid.NewGuid()).ToList();
            StartNewQuiz(ShuffledQuestions);
            RaisePropertyChanged(nameof(ShuffledQuestions));
        }

        private async void AnswerSelected(object? parameter)
        {
            if (!_canAnswer) return;
            _canAnswer = false;

            string selectedAnswer = parameter as string;
            if (CurrentQuestion != null && selectedAnswer != null)
            {
                ResetButtonColors();

                var playerAnswer = new PlayerAnswer
                {
                    QuestionText = CurrentQuestion.Query,
                    SelectedAnswer = selectedAnswer,
                    CorrectAnswer = CurrentQuestion.CorrectAnswer
                };
                Answers.Add(playerAnswer);

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
                await ShowAnswerStatistics();
                ShowPercentages = true;
                await Task.Delay(2000);
                await NextQuestion();
            }
            _canAnswer = true;
        }

        private async Task ShowAnswerStatistics()
        {
            var playerResultCollection = _mongoDBService.GetPlayerResultCollection();
            var filter = Builders<PlayerResult>.Filter.Eq("QuestionPackId", QuestionPackId);
            var playerResults = await playerResultCollection.Find(filter).ToListAsync();

            var answerCounts = new Dictionary<string, int>
            {
                {Answer1, 0 },
                {Answer2, 0 },
                {Answer3, 0 },
                {Answer4, 0 }
            };

            int totalAnswers = 0;

            foreach (var result in playerResults)
            {
                foreach (var answer in result.Answers)
                {
                    if (answer.QuestionText == CurrentQuestion.Query && answerCounts.ContainsKey(answer.SelectedAnswer))
                    {
                        answerCounts[answer.SelectedAnswer]++;
                        totalAnswers++;
                    }
                }
            }
            Option1Percentage = totalAnswers > 0 ? (double)answerCounts[Answer1] / totalAnswers * 100 : 0;
            Option2Percentage = totalAnswers > 0 ? (double)answerCounts[Answer2] / totalAnswers * 100 : 0;
            Option3Percentage = totalAnswers > 0 ? (double)answerCounts[Answer3] / totalAnswers * 100 : 0;
            Option4Percentage = totalAnswers > 0 ? (double)answerCounts[Answer4] / totalAnswers * 100 : 0;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_mainWindowViewModel.IsPlayMode && !IsStartButtonVisible)
            {
                Debug.WriteLine($"Time Remaining: {TimeRemaining}");
                if (TimeRemaining > 0)
                {
                    TimeRemaining--;
                    _sessionTimer = _sessionTimer.Add(TimeSpan.FromSeconds(1));
                }
                else
                {
                    ShowCorrectAnswer();
                    Task.Delay(1000).ContinueWith(t => NextQuestion());
                }
            }
        }

        public async Task NextQuestion()
        {
            if (CurrentQuestionNumber < TotalQuestions)
            {
                ResetButtonColors();
                CurrentQuestionNumber++;
                CurrentQuestion = ShuffledQuestions[CurrentQuestionNumber - 1];

                TimeRemaining = _mainWindowViewModel.ActivePack.TimeLimitInSeconds;
                RaisePropertyChanged(nameof(TimeRemaining));
                _questionTimer.Start();
                ShowPercentages = false;
            }
            else
            {
                _questionTimer.Stop();
                IsQuizFinished = true;
                await EndQuizPlayerResult();
                Debug.WriteLine("Quiz finished");
                RaisePropertyChanged(TotalScore);
                ShowPercentages = false;
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

        private string[] GetShuffledAnswers(Question question)
        {
            var answers = new List<string>
            {
                question.IncorrectAnswer1,
                question.IncorrectAnswer2,
                question.IncorrectAnswer3,
                question.CorrectAnswer
            };

            return answers.OrderBy(a => _random.Next()).ToArray();
        }

        public string QuestionOfTotalQuestion => $"Question {CurrentQuestionNumber} of {TotalQuestions}";

        public TimeSpan SessionTimer
        {
            get => _sessionTimer;
            set
            {
                _sessionTimer = value;
                RaisePropertyChanged();
            }
        }

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                RaisePropertyChanged();
            }
        }

        public ObjectId QuestionPackId
        {
            get => _questionPackId;
            set
            {
                _questionPackId = value;
                RaisePropertyChanged(nameof(QuestionPackId));
            }
        }

        public string QuestionPackName
        {
            get => _questionPackName;
            set
            {
                _questionPackName = value;
                RaisePropertyChanged(nameof(QuestionPackName));
            }
        }

        public List<PlayerAnswer> Answers
        {
            get => _answers;
            set
            {
                _answers = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PlayerResult> Top5PlayerResult
        {
            get => _top5PlayerResult;
            set
            {
                _top5PlayerResult = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> CurrentAnswerOptions { get; private set; }

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
                        CurrentAnswerOptions = new ObservableCollection<string>(_currentAnswers);
                    }
                    else
                    {
                        _currentAnswers = Array.Empty<string>();
                        CurrentAnswerOptions = new ObservableCollection<string>();
                    }

                    RaisePropertyChanged(nameof(Answer1));
                    RaisePropertyChanged(nameof(Answer2));
                    RaisePropertyChanged(nameof(Answer3));
                    RaisePropertyChanged(nameof(Answer4));
                }
            }
        }

        public string Answer1 => _currentAnswers?.Length > 0 ? _currentAnswers[0] : string.Empty;
        public string Answer2 => _currentAnswers?.Length > 1 ? _currentAnswers[1] : string.Empty;
        public string Answer3 => _currentAnswers?.Length > 2 ? _currentAnswers[2] : string.Empty;
        public string Answer4 => _currentAnswers?.Length > 3 ? _currentAnswers[3] : string.Empty;

        public int CurrentQuestionNumber
        {
            get => _currentQuestionNumber;
            set
            {
                _currentQuestionNumber = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(QuestionOfTotalQuestion));
            }
        }

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

        public string EndOfQuizMessage
        {
            get => _endOfQuizMessage;
            set
            {
                _endOfQuizMessage = value;
                RaisePropertyChanged(nameof(EndOfQuizMessage));
            }
        }

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
                }
            }
        }

        public bool IsStartButtonVisible
        {
            get => _isStartButtonVisible;
            set
            {
                _isStartButtonVisible = value;
                RaisePropertyChanged(nameof(IsStartButtonVisible));
            }
        }

        public bool IsQuizRunning
        {
            get => _isQuizRunning;
            set
            {
                _isQuizRunning = value;
                RaisePropertyChanged(nameof(IsQuizRunning));
            }
        }
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

        public string TotalScore => $"{CorrectAnswerCount} of {TotalQuestions} correct!";

        public SolidColorBrush Option1StatusColor
        {
            get => _option1StatusColor;
            set
            {
                _option1StatusColor = value;
                RaisePropertyChanged(nameof(Option1StatusColor));
            }
        }
        public SolidColorBrush Option2StatusColor
        {
            get => _option2StatusColor;
            set
            {
                _option2StatusColor = value;
                RaisePropertyChanged(nameof(Option2StatusColor));
            }
        }
        public SolidColorBrush Option3StatusColor
        {
            get => _option3StatusColor;
            set
            {
                _option3StatusColor = value;
                RaisePropertyChanged(nameof(Option3StatusColor));
            }
        }
        public SolidColorBrush Option4StatusColor
        {
            get => _option4StatusColor;
            set
            {
                _option4StatusColor = value;
                RaisePropertyChanged(nameof(Option4StatusColor));
            }
        }
        public double Option1Percentage
        {
            get => _option1Percentage;
            set
            {
                _option1Percentage = value;
                RaisePropertyChanged(nameof(Option1Percentage));
            }
        }
        public double Option2Percentage
        {
            get => _option2Percentage;
            set
            {
                _option2Percentage = value;
                RaisePropertyChanged(nameof(Option2Percentage));
            }
        }
        public double Option3Percentage
        {
            get => _option3Percentage;
            set
            {
                _option3Percentage = value;
                RaisePropertyChanged(nameof(Option3Percentage));
            }
        }
        public double Option4Percentage
        {
            get => _option4Percentage;
            set
            {
                _option4Percentage = value;
                RaisePropertyChanged(nameof(Option4Percentage));
            }
        }
        public bool ShowPercentages
        {
            get => _showPercentages;
            set
            {
                _showPercentages = value;
                RaisePropertyChanged(nameof(ShowPercentages));
            }
        }
    }
}