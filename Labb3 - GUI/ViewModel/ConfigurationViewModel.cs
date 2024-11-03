using Labb3___GUI.Command;
using Labb3___GUI.Model;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Labb3___GUI.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {

        private readonly MainWindowViewModel? mainWindowViewModel;
        public DelegateCommand AddButtonCommand { get;}
        public DelegateCommand RemoveButtonCommand { get;}

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

        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        private Question? _activeQuestion;
        public Question? ActiveQuestion { get => _activeQuestion; set { _activeQuestion = value; RaisePropertyChanged();
                VisibilityMode = (_activeQuestion != null) ? Visibility.Visible : Visibility.Hidden;
                RemoveButtonCommand.RaiseCanExecuteChanged();
            }
        }
        private bool RemoveActiveButton(object? arg)      //dont think it does anything, can get removed?
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
            ActiveQuestion = ActivePack?.Questions.LastOrDefault();
            RemoveButtonCommand.RaiseCanExecuteChanged();
        }
        private bool CanRemoveQuestion(object? arg) => ActiveQuestion != null;


        private Visibility _visibility;
        public Visibility VisibilityMode { get => _visibility; set { _visibility = value; RaisePropertyChanged(); } }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled;  set { _isEnabled = value; RaisePropertyChanged();; } }
        public void SaveQuestionPack(string filePath)
        {
            if (ActivePack != null)
            {
                // Serialize the QuestionPack to JSON
                string json = JsonSerializer.Serialize(ActivePack, new JsonSerializerOptions { WriteIndented = true });

                // Write the JSON to a file
                File.WriteAllText(filePath, json);
            }
        }

       /* public void LoadQuestionPack(string filePath)
        {
            if (File.Exists(filePath))
            {
                // Read the JSON from the file
                string json = File.ReadAllText(filePath);

                // Deserialize the JSON back to a QuestionPack
                var loadedPack = JsonSerializer.Deserialize<QuestionPack>(json);

                if (loadedPack != null)
                {
                    ActivePack = loadedPack;
                    ActiveQuestion = ActivePack.Questions.FirstOrDefault();
                }
            }
        }*/


    }
}
