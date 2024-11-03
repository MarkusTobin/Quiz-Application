using Labb3___GUI.Command;
using Labb3___GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace Labb3___GUI.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        public void StartNewQuiz(IEnumerable<Question> questions)
        {
            if (questions == null || !questions.Any())
            {
                // Handle the case where there are no questions
                IsQuizFinished = true;
                EndOfQuizMessage = "No questions available.";
                return;
            }

            // Shuffle questions and set up the quiz
            ShuffledQuestions = questions.OrderBy(q => Guid.NewGuid()).ToList();
            TotalQuestions = ShuffledQuestions.Count;
            CurrentQuestionNumber = 1;
            CurrentQuestion = ShuffledQuestions.FirstOrDefault();
            CorrectAnswerCount = 0;

            // Reset timer for the new quiz
            TimeRemaining = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            timer.Start();
        }
        private readonly MainWindowViewModel? mainWindowViewModel;
        private DispatcherTimer timer;


        //Fixa commands
        public DelegateCommand AnswerCommand { get; }

        public DelegateCommand RestartQuizCommand { get; }

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
        //
        private Question _currentQuestion; // maybe QuiestionViewmodel instead of Question?
        public Question CurrentQuestion    // maybe QuiestionViewmodel instead of Question?
        { 
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                RaisePropertyChanged(nameof(CurrentQuestion));
            }
        }

        private int _currentQuestionNumber;
        public int CurrentQuestionNumber
        {
            get => _currentQuestionNumber; set
            {
                _currentQuestionNumber = value;
                RaisePropertyChanged(nameof(CurrentQuestionNumber));
            }
        }

        private int _totalQuestions;
        public int TotalQuestions
        {
            get => _totalQuestions;
            set
            {
                _totalQuestions = value;
                RaisePropertyChanged(nameof(TotalQuestions));
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
                RaisePropertyChanged(nameof(_timeRemaining));
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
                _isQuizFinished = value;
                RaisePropertyChanged(nameof(IsQuizFinished));
            }
        }

        private int _correctAnswerCount;
        public int CorrectAnswerCount
        {
            get => _correctAnswerCount;
            set
            {
                _correctAnswerCount = value;
                RaisePropertyChanged(nameof(CorrectAnswerCount));
            }
        }
        //
        // Peta in lite commands här? tex välja svar
        // Peta in lite commands här? tex välja restarta game
        // Peta in lite commands här? tex välja 
        public DelegateCommand UpdateButtonCommand { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            if (mainWindowViewModel?.ActivePack != null && mainWindowViewModel.ActivePack.Questions != null)
            {
                ShuffledQuestions = mainWindowViewModel.ActivePack.Questions.OrderBy(q => Guid.NewGuid()).ToList();
                TotalQuestions = ShuffledQuestions.Count;
            }

            else
            {
                ShuffledQuestions = new List<Question>(); // or handle as needed
                TotalQuestions = 0; // No questions available
            }
            CurrentQuestionNumber = 1;
            CurrentQuestion = ShuffledQuestions.FirstOrDefault();
            CorrectAnswerCount = 0;

            TimeRemaining = mainWindowViewModel.ActivePack?.TimeLimitInSeconds ?? 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;

            AnswerCommand = new DelegateCommand(AnswerSelected);

        }

        private void AnswerSelected(object? obj)
        {
             string selectedAnswer = obj as string;
            if (CurrentQuestion != null && selectedAnswer != null)
            {
                // Reset button colors
                ResetButtonColors();

                // Check if the answer is correct
                if (CurrentQuestion.CorrectAnswer == selectedAnswer)
                {
                    CorrectAnswerCount++;
                    SetButtonColor(selectedAnswer, Brushes.Green); // Set the selected button to green
                }
                else
                {
                    // Show correct answer
                    ShowCorrectAnswer();
                    SetButtonColor(selectedAnswer, Brushes.Red); // Set the selected button to red
                    SetButtonColor(CurrentQuestion.CorrectAnswer, Brushes.Green); // Highlight correct answer
                }

                // Move to the next question
                NextQuestion();
            }
        }
        private void TimerTick(object sender, EventArgs e)
        {
            if (TimeRemaining > 0) 
            {
                timer.Stop();
                EndOfQuizMessage = "Time is up, guess quicker next time!";
                ShowCorrectAnswer();
                NextQuestion();
            }
        }
        public void NextQuestion()
        {
            if (CurrentQuestionNumber < TotalQuestions)
            {
                CurrentQuestionNumber++;
                CurrentQuestion = ShuffledQuestions[CurrentQuestionNumber-1];
                TimeRemaining = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
                timer.Start();
            }
            else
            {
                IsQuizFinished = true;
                timer.Stop();
                EndOfQuizMessage = $"Quiz finsished! You got {CorrectAnswerCount} out of {TotalQuestions} correct";
            }
        }

        private void ShowCorrectAnswer()
        {
            if (AnswerSelected == ShowCorrectAnswer)
            {
                //Logic för att visa färg för rätt fel svar
                Thread.Sleep(1000);
                CorrectAnswerCount++;
                NextQuestion();
            }
            else
            {
                //Logic för att visa färg för rätt fel svar
                Thread.Sleep(1000);
                NextQuestion();
            }
        }
        private void ResetButtonColors()
        {
            Option1StatusColor = Brushes.White;
            Option2StatusColor = Brushes.White;
            Option3StatusColor = Brushes.White;
            Option4StatusColor = Brushes.White;
        }
        // byta namn på _option1StatusColor etc till CorrectAnswer --> IncorrectAnswer1-3
        private void SetButtonColor(string answer, SolidColorBrush color)
        {
            if (answer == CurrentQuestion.CorrectAnswer) Option1StatusColor = color;
            else if (answer == CurrentQuestion.IncorrectAnswer1) Option2StatusColor = color;
            else if (answer == CurrentQuestion.IncorrectAnswer2) Option3StatusColor = color;
            else if (answer == CurrentQuestion.IncorrectAnswer3) Option4StatusColor = color;
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
