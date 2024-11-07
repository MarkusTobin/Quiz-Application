using Labb3___GUI.Model;
using System.Collections.ObjectModel;

namespace Labb3___GUI.ViewModel
{
    internal class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack _model;
        public QuestionPackViewModel(QuestionPack model)
        {
            _model = model;
            Questions = new ObservableCollection<Question>(model.Questions ?? new List<Question>());

        }
        public QuestionPack Model => _model;
        public string DisplayText => $"{Name} ({Difficulty})"; //nytt
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
            get => (int)_model.Difficulty; // Convert Difficulty enum to int index
            set
            {
                _model.Difficulty = (Difficulty)value; // Convert index back to Difficulty enum
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Difficulty)); // Notify that Difficulty also changed
                RaisePropertyChanged(nameof(DisplayText)); // Update DisplayText if it depends on Difficulty
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