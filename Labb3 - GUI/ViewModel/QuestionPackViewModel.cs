using Labb3___GUI.Model;
using System.Collections.ObjectModel;

namespace Labb3___GUI.ViewModel
{
    internal class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack model;

        public QuestionPackViewModel(QuestionPack model)
        {
            this.model = model;
            this.Questions = new ObservableCollection<Question>(model.Questions);
        }

        public string Name { 
            get=> model.Name;
            set
            {
                model.Name = value;
            }
        }
        public Difficulty Difficulty {
            get => model.Difficulty;
            set
            {
                model.Difficulty = value;
            }
            }
        public int TimeLimitInSeconds
        {
            get => model.TimeLimitInSeconds;
            set
            {
                model.TimeLimitInSeconds = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Question> Questions { get; }
    }
}
