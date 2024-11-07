using Labb3___GUI.Command;
using Labb3___GUI.Dialogs;
using Labb3___GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;

namespace Labb3___GUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {

        public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new ObservableCollection<QuestionPackViewModel>(); //
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public PlayerViewModel PlayerViewModel { get; }
        private QuestionPackViewModel? _activePack;
        public DelegateCommand StartQuizCommand { get; }
        public DelegateCommand SetConfigModeCommand { get; }
        public DelegateCommand SetPlayModeCommand { get; }
        //new changes-----------------------------------------------------------------------------
        public DelegateCommand AddPackCommand { get; }    //nytt inlagt
        public DelegateCommand RemovePackCommand { get; } //nytt inlagt
        public DelegateCommand EditPackCommand { get; }   //nytt inlagt
        public DelegateCommand SelectPackCommand { get; } //nytt inlagt
        public DelegateCommand CloseDialogCommand { get; }

        private readonly PlayerViewModel _playerViewModel;
        private ObservableCollection<Question> _questions;
        public MainWindowViewModel()
        {

            ConfigurationViewModel = new ConfigurationViewModel(this);

            var defaultPack = new QuestionPack("My Question Pack", Difficulty.Medium, 30);


            var defaultQuestion = new Question("What is 2 + 2?", "4", "3", "5", "6");
            var defaultQuestion2 = new Question("What is 4 + 4?", "8", "3", "5", "6");
            defaultPack.Questions.Add(defaultQuestion);
            defaultPack.Questions.Add(defaultQuestion2);


            var defaultPackViewModel = new QuestionPackViewModel(defaultPack);

            Packs = new ObservableCollection<QuestionPackViewModel> { defaultPackViewModel };
            ActivePack = defaultPackViewModel;

            PlayerViewModel = new PlayerViewModel(this);
            if (ActivePack?.Questions != null && ActivePack.Questions.Any())
            {
                PlayerViewModel.StartNewQuiz(ActivePack.Questions.ToList());
            }
            else
            {
                Debug.WriteLine("Error: ActivePack does not contain any questions.");
            }





            AddPackCommand = new DelegateCommand(AddPack);
            RemovePackCommand = new DelegateCommand(RemovePack, CanRemovePack);
            EditPackCommand = new DelegateCommand(EditPack, CanEditPack);
            SelectPackCommand = new DelegateCommand(SelectPack);
            CloseDialogCommand = new DelegateCommand(CloseDialogWindow);
            StartQuizCommand = new DelegateCommand(StartQuiz);
            SetConfigModeCommand = new DelegateCommand(_ => SetConfigMode());
            SetPlayModeCommand = new DelegateCommand(_ => SetPlayMode());//här?

            IsConfigMode = true;
            IsPlayMode = false; // ta bort?
        }

        //gammalt
        private void SelectPack(object selectedPackObj)
        {
            if (selectedPackObj is QuestionPackViewModel selectedPack)
            {
                ActivePack = selectedPack;
                //gammalt
                //mainWindowViewModel.ActivePack = selectedPack;
              //  SetActivePack(selectedPack.Model);
            }
        }
        // Assuming you have a method to change the active pack
        //gammalt
        public void SetActivePack(QuestionPack newPack)
        {
            ActivePack = new QuestionPackViewModel(newPack);  // Ensure this is correctly updating the active pack
            RaisePropertyChanged(nameof(ActivePack));
            PlayerViewModel.StartNewQuiz(ActivePack.Questions.ToList());  // Refresh the quiz
        }

        public void CloseDialogWindow(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false; // Or true, depending on whether you want to cancel or save
                window.Close();
            }
        }

        private void AddPack(object parameter)
        {

            var newPack = new QuestionPack("Default name", Difficulty.Medium, 30);
            var newPackViewModel = new QuestionPackViewModel(newPack);


            var dialog = new CreateNewPackDialog()
            {
                DataContext = newPackViewModel
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {

                Packs.Add(newPackViewModel);
                ActivePack = newPackViewModel;
                RaisePropertyChanged(nameof(Packs));
            }
        }

        private void RemovePack(object parameter)
        {
            if (ActivePack != null)
            {
                Packs.Remove(ActivePack);
                ActivePack = Packs.FirstOrDefault();
                RaisePropertyChanged(nameof(ActivePack));
            }
        }

        private bool CanRemovePack(object parameter)
        {
            return ActivePack != null;
        }

        public void EditPack(object parameter)
        {
            if (ActivePack != null)
            {
                var dialog = new PackOptionsDialog
                {
                    DataContext = ActivePack
                };

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    RaisePropertyChanged(nameof(ActivePack));
                }
            }
        }

        private bool CanEditPack(object parameter)
        {
            return ActivePack != null;
        }

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {

                _activePack = value;
                RaisePropertyChanged();
                ConfigurationViewModel.RaisePropertyChanged(nameof(ConfigurationViewModel.ActivePack));
                ConfigurationViewModel.ActiveQuestion = ActivePack?.Questions.FirstOrDefault();
            }
        }
        //public Window ParentWindow => Application.Current.MainWindow; // is this usefull?
        private readonly MainWindowViewModel? mainWindowViewModel;
        private bool _isPlayMode;
        public bool IsPlayMode
        {
            get => _isPlayMode;
            set
            {
                _isPlayMode = value;
                RaisePropertyChanged();
                if (_isPlayMode)
                {
                    StartGame();
                }
            }
        }
        private void StartGame()
        {
            PlayerViewModel.StartNewQuiz(ActivePack.Questions.ToList());
        }
        private bool _isConfigMode;
        public bool IsConfigMode
        {
            get => _isConfigMode;
            set
            {
                _isConfigMode = value;
                RaisePropertyChanged();
            }
        }
        private void StartQuiz(object parameter)
        {
            if (ActivePack == null || !ActivePack.Questions.Any())
            {
                return;
            }
            IsPlayMode = true;
            IsConfigMode = false;
            PlayerViewModel.StartNewQuiz(ActivePack.Questions.ToList());
        }
        private void SetConfigMode()
        {
            IsConfigMode = true;
            IsPlayMode = false;
        }

        private void SetPlayMode()
        {
            IsPlayMode = true;
            IsConfigMode = false;
        }
        //gammalt
        /*public void AddQuestion(Question newQuestion)
        {
            ActivePack.Questions.Add(newQuestion);  // Add new question
            PlayerViewModel.StartNewQuiz(ActivePack.Questions);  // Refresh the quiz with the updated list
        }*/

    }
}
