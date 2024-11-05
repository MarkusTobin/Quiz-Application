using Labb3___GUI.Command;
using Labb3___GUI.Dialogs;
using Labb3___GUI.Model;
using System.Collections.ObjectModel;
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


        //Pilla med pack
        public DelegateCommand EditPackCommand { get;}
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

        //Pilla med pack slut
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
        private ObservableCollection<Question> questions; //nytt inlagt, ta bort?
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        public ObservableCollection<Question> Questions
        {
            get => questions;
            set
            {
                questions = value;
                RaisePropertyChanged(nameof(Questions));
            }
        }

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

        private bool CanEditPack(object arg)
        {
            return ActivePack != null;
        }
    }
}
