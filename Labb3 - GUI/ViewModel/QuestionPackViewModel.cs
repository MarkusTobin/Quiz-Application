using Labb3___GUI.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Labb3___GUI.ViewModel
{
    internal class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack _model;
        public QuestionPackViewModel(QuestionPack model)
        {
            _model = model;
            Questions = new ObservableCollection<Question>(model.Questions ?? new List<Question>());

            Questions.CollectionChanged += OnQuestionsChanged;

        }

        private void OnQuestionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _model.Questions.Clear();
            foreach (var question in Questions)
            {
                _model.Questions.Add(question);
            }
        }

        public QuestionPack Model => _model;
        public string DisplayText => $"{Name} ({Difficulty})";
        public string Name
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayText));
            }
        }
        public Difficulty Difficulty
        {
            get => _model.Difficulty;
            set
            {
                _model.Difficulty = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayText));
            }

        }
        public int DifficultyIndex
        {
            get => (int)_model.Difficulty;
            set
            {
                _model.Difficulty = (Difficulty)value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Difficulty));
                RaisePropertyChanged(nameof(DisplayText));
            }
        }
        public int TimeLimitInSeconds
        {
            get => _model.TimeLimitInSeconds;
            set
            {
                _model.TimeLimitInSeconds = value;
                RaisePropertyChanged(nameof(TimeLimitInSeconds));
            }
        }
        public ObservableCollection<Question> Questions { get; }
    }
}