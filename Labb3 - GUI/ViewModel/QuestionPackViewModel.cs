using Labb3___GUI.Command;
using Labb3___GUI.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Labb3___GUI.ViewModel
{
    internal class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack _questionPack;
        private readonly CategoryViewModel _categoryViewModel;
        private ObservableCollection<string> _categories;

        public ObservableCollection<Question> Questions { get; }
        public ObservableCollection<string> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                RaisePropertyChanged();
                if (_categories != null && _categories.Count > 0)
                {
                    if (SelectedCategory != null)
                    {
                        return;
                    }
                    else
                    SelectedCategory = _categories[0];
                }
            }
        }
        public DelegateCommand OpenEditCategoriesCommand { get; set; }

        public QuestionPackViewModel(QuestionPack questionPack)
        {
            _categoryViewModel = new CategoryViewModel();
            _questionPack = questionPack;
            Questions = new ObservableCollection<Question>(questionPack.Questions ?? new List<Question>());

            Questions.CollectionChanged += OnQuestionsChanged;
        }

        private void OnQuestionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _questionPack.Questions.Clear();
            foreach (var question in Questions)
            {
                _questionPack.Questions.Add(question);
            }
        }

        public QuestionPack QuestionPack => _questionPack;
        public string DisplayText => $"{Name} ({Difficulty})";
        public string Name
        {
            get => _questionPack.Name;
            set
            {
                _questionPack.Name = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayText));
            }
        }
        public Difficulty Difficulty
        {
            get => _questionPack.Difficulty;
            set
            {
                _questionPack.Difficulty = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DisplayText));
            }

        }
        public int DifficultyIndex
        {
            get => (int)_questionPack.Difficulty;
            set
            {
                _questionPack.Difficulty = (Difficulty)value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Difficulty));
                RaisePropertyChanged(nameof(DisplayText));
            }
        }
        public int TimeLimitInSeconds
        {
            get => _questionPack.TimeLimitInSeconds;
            set
            {
                _questionPack.TimeLimitInSeconds = value;
                RaisePropertyChanged(nameof(TimeLimitInSeconds));
            }
        }
        public string SelectedCategory
        {
            get => _questionPack.Category;
            set
            {
                _questionPack.Category = value;
                RaisePropertyChanged();
            }
        }
    }
}