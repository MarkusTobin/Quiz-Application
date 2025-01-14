using Labb3___GUI.Command;
using Labb3___GUI.Dialogs;
using Labb3___GUI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Labb3___GUI.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;
        public DelegateCommand AddButtonCommand { get; }
        public DelegateCommand RemoveButtonCommand { get; }

        public DelegateCommand EditPackCommand { get; }
        public void EditPack(object? parameter)
        {
            if (ActivePack != null)
            {
                var dialog = new PackOptionsDialog();
                var dialogViewModel = new QuestionPack
                {
                    Name = ActivePack.Name,
                    Difficulty = ActivePack.Difficulty,
                    TimeLimitInSeconds = ActivePack.TimeLimitInSeconds
                };

                dialog.DataContext = dialogViewModel;

                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    ActivePack.Name = dialogViewModel.Name;
                    ActivePack.Difficulty = dialogViewModel.Difficulty;
                    ActivePack.TimeLimitInSeconds = dialogViewModel.TimeLimitInSeconds;
                }
            }
        }

        public ConfigurationViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            VisibilityMode = Visibility.Hidden;

            IsEnabled = true;
            AddButtonCommand = new DelegateCommand(AddButton);
            RemoveButtonCommand = new DelegateCommand(RemoveButton, CanRemoveQuestion);
            EditPackCommand = new DelegateCommand(EditPack, CanEditPack);

            ActivePack?.Questions.Add(new Question("Question abc", "a", "b", "c", "d"));
            ActiveQuestion = ActivePack?.Questions.FirstOrDefault();
        }
        private ObservableCollection<Question> questions;
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        public ObservableCollection<Question> Questions
        {
            get => questions;
            set
            {
                questions = value;
                RaisePropertyChanged(nameof(Questions));
                Debug.WriteLine("Questions updated:", questions.Count);
            }
        }

        private Question? _activeQuestion;
        public Question? ActiveQuestion
        {
            get => _activeQuestion; set
            {
                _activeQuestion = value; RaisePropertyChanged();
                VisibilityMode = (_activeQuestion != null) ? Visibility.Visible : Visibility.Hidden;
                RemoveButtonCommand.RaiseCanExecuteChanged();
            }
        }
        private bool RemoveActiveButton(object? arg)
        {
            return IsEnabled = true;
        }
        private void AddButton(object? parameter)
        {
            VisibilityMode = Visibility.Visible;

            var newQuestion = new Question("", "", "", "", "");
            ActivePack?.Questions.Add(newQuestion);
            ActiveQuestion = newQuestion;
            RemoveButtonCommand.RaiseCanExecuteChanged();

        }
        private void RemoveButton(object? parameter)
        {
            ActivePack?.Questions.Remove(ActiveQuestion);
            ActiveQuestion = ActivePack?.Questions.LastOrDefault();
            RemoveButtonCommand.RaiseCanExecuteChanged();
        }

        private bool CanRemoveQuestion(object? parameter) => ActiveQuestion != null;


        private Visibility _visibility;
        public Visibility VisibilityMode { get => _visibility; set { _visibility = value; RaisePropertyChanged(); } }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set { _isEnabled = value; RaisePropertyChanged(); ; } }
        private bool CanEditPack(object arg)
        {
            return ActivePack != null;
        }
    }
}