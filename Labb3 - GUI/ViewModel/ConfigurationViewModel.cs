using Labb3___GUI.Command;
using Labb3___GUI.Model;
using System.Windows;

namespace Labb3___GUI.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;
        public DelegateCommand AddButtonCommand { get;}
        public DelegateCommand RemoveButtonCommand { get;}
        private bool CanRemoveQuestion(object? arg)
        {
            return ActiveQuestion != null;
        }
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            VisibilityMode = Visibility.Hidden;
            

            IsEnabled = true;

            AddButtonCommand = new DelegateCommand(AddButton);
            RemoveButtonCommand = new DelegateCommand(RemoveButton, CanRemoveQuestion);

            ActivePack?.Questions.Add(new Question("Question abc", "a", "b", "c", "d"));
            ActiveQuestion = ActivePack?.Questions.FirstOrDefault();
        }
        private bool RemoveActiveButton(object? arg)
        {
            return IsEnabled = true;
        }
        private void AddButton(object? obj) 
        {
            VisibilityMode = Visibility.Visible;


            var newQuestion = new Question("", "", "", "", "");
            ActivePack?.Questions.Add(newQuestion);
            ActiveQuestion = newQuestion;
            RemoveButtonCommand.RaiseCanExecuteChanged();

        }
        private void RemoveButton(object? obj)
        {
            ActivePack?.Questions.Remove(ActiveQuestion);
            RemoveButtonCommand.RaiseCanExecuteChanged();
            VisibilityMode = Visibility.Hidden;

        }
        private Question? _activeQuestion;
        public Question? ActiveQuestion { get => _activeQuestion; set { _activeQuestion = value; RaisePropertyChanged();
                VisibilityMode = (_activeQuestion != null) ? Visibility.Visible : Visibility.Hidden;
                RemoveButtonCommand.RaiseCanExecuteChanged();
            }
        }


        private Visibility _visibility;
        public Visibility VisibilityMode { get => _visibility; set { _visibility = value; RaisePropertyChanged(); } }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled;  set { _isEnabled = value; RaisePropertyChanged();; } }
    }
}
