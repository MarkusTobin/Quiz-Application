using Labb3___GUI.Command;
using Labb3___GUI.Dialogs;
using Labb3___GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
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
        //public Window ParentWindow => Application.Current.MainWindow; // is this usefull?
        private bool _isPlayMode;
        public bool IsPlayMode
        {
            get => _isPlayMode;
            set
            {
                _isPlayMode = value;
                RaisePropertyChanged();
            }
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
        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);
            PlayerViewModel = new PlayerViewModel(this);
            Packs = new ObservableCollection<QuestionPackViewModel>();
            ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
            Packs.Add(ActivePack);

            AddPackCommand = new DelegateCommand(AddPack);                          //nytt
            RemovePackCommand = new DelegateCommand(RemovePack, CanRemovePack);     //nytt
            EditPackCommand = new DelegateCommand(EditPack, CanEditPack);           //nytt
            SelectPackCommand = new DelegateCommand(SelectPack);

            CloseDialogCommand = new DelegateCommand(CloseDialogWindow);

            StartQuizCommand = new DelegateCommand(StartQuiz);
            SetConfigModeCommand = new DelegateCommand(_ => SetConfigMode());
            SetPlayModeCommand = new DelegateCommand(_ => SetPlayMode());

            IsConfigMode = true; // might not me needed
            IsPlayMode = false;  // might not me needed
        }
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new ObservableCollection<QuestionPackViewModel>(); //
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public PlayerViewModel PlayerViewModel { get; }

        public DelegateCommand StartQuizCommand { get; }
        public DelegateCommand SetConfigModeCommand { get; }
        public DelegateCommand SetPlayModeCommand { get; }
        //new changes-----------------------------------------------------------------------------
        public DelegateCommand AddPackCommand { get; }    //nytt inlagt
        public DelegateCommand RemovePackCommand { get; } //nytt inlagt
        public DelegateCommand EditPackCommand { get; }   //nytt inlagt
        public DelegateCommand SelectPackCommand { get; } //nytt inlagt
        public DelegateCommand CloseDialogCommand { get; }

        private void SelectPack(object selectedPackObj)
        {
            if (selectedPackObj is QuestionPackViewModel selectedPack)
            {
                ActivePack = selectedPack;
                // Additional logic could be added here to update UI or load questions if needed
            }
        }
        public void CloseDialogWindow(object parameter)
        {
            var window = (Window)parameter;
            window.Close();
        }

        private void AddPack(object parameter)
        {
            var dialog = new CreateNewPackDialog();
            var newPackViewModel = new QuestionPackViewModel(new QuestionPack()); // Initialize with an empty model
            dialog.DataContext = newPackViewModel;

            // Show the dialog and check if the dialog result is true
            bool? result = dialog.ShowDialog();
            if (result == true) // Only add the new pack if the dialog was confirmed
            {
                // Only add to the Packs collection if the dialog was accepted
                Packs.Add(newPackViewModel);
                ActivePack = newPackViewModel; // Optionally select the new pack
                RaisePropertyChanged(nameof(Packs));
            }
        }

        /*       private void CreateNewPackDialog(object parameter)
               {
                   var newPack = new QuestionPack(); // Use default constructor
                   var viewModel = new QuestionPackViewModel(newPack);
                   var dialog = new CreateNewPackDialog
                   {
                       DataContext = viewModel
                   };

                   bool? result = dialog.ShowDialog();

                   if (result == true)
                   {
                       var createdPack = viewModel.Model;
                       var createdPackViewModel = new QuestionPackViewModel(createdPack); 
                       Packs.Add(createdPackViewModel);
                       RaisePropertyChanged(nameof(Packs));
                   }
               }
       */
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
        //new changes-----------------------------------------------------------------------------
       

        private QuestionPackViewModel? _activePack;
        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {

                _activePack = value;
                RaisePropertyChanged();
                ConfigurationViewModel.RaisePropertyChanged(nameof(ConfigurationViewModel.ActivePack));
                ConfigurationViewModel.ActiveQuestion = ActivePack?.Questions.FirstOrDefault();
                RaisePropertyChanged(nameof(ActivePack)); 
                RaisePropertyChanged(nameof(ActivePack.Name));
               // ((DelegateCommand)RemovePackCommand).RaiseCanExecuteChanged();                //Kolla om det ska ha (Delgatecommand) eller ej //nytt också
               // ((DelegateCommand)EditPackCommand).RaiseCanExecuteChanged(); //Kolla om det ska ha (Delgatecommand) eller ej //nytt också
            }
        }
        private void StartQuiz(object obj)
        {
            if (ActivePack == null || !ActivePack.Questions.Any())
            {
                return;
            }
            IsPlayMode = true;
            IsConfigMode = false;
            PlayerViewModel.StartNewQuiz(ActivePack.Questions);
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
    }
}