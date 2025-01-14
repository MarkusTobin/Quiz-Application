using Labb3___GUI.Command;
using Labb3___GUI.Dialogs;
using Labb3___GUI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Labb3___GUI.MongoDB;
using MongoDB.Driver;


namespace Labb3___GUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new ObservableCollection<QuestionPackViewModel>();
        public ConfigurationViewModel ConfigurationViewModel { get; }
        public PlayerViewModel PlayerViewModel { get; }
        private QuestionPackViewModel? _activePack;

        public DelegateCommand StartQuizCommand { get; }
        public DelegateCommand SetConfigModeCommand { get; }
        public DelegateCommand SetPlayModeCommand { get; }
        public DelegateCommand AddPackCommand { get; }
        public DelegateCommand RemovePackCommand { get; }
        public DelegateCommand EditPackCommand { get; }
        public DelegateCommand SelectPackCommand { get; }
        public DelegateCommand CloseDialogCommand { get; }
        public DelegateCommand ToggleFullScreenCommand { get; set; }
        public DelegateCommand ExitAndSaveCommand { get; }

        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);
            Task.Run(async () => await LoadFromMongoDB()).Wait();

            Debug.WriteLine(Packs.Count);
            Debug.WriteLine(Packs.Count);
            Debug.WriteLine(Packs.Count);


            if (Packs == null || Packs.Count == 0)
            {

                var defaultPack = new QuestionPack("My Question Pack", Difficulty.Medium, 30);
                var defaultQuestion = new Question("What is 2 + 2?", "4", "3", "5", "6");
                var defaultQuestion2 = new Question("What is 4 + 4?", "8", "3", "5", "6");
                defaultPack.Questions.Add(defaultQuestion);
                defaultPack.Questions.Add(defaultQuestion2);

                var defaultPackViewModel = new QuestionPackViewModel(defaultPack);
                Packs = new ObservableCollection<QuestionPackViewModel> { defaultPackViewModel };
                ActivePack = defaultPackViewModel;
            }

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
            SetPlayModeCommand = new DelegateCommand(_ => SetPlayMode());
            ToggleFullScreenCommand = new DelegateCommand(ToggleFullScreen);
            ExitAndSaveCommand = new DelegateCommand(ExitAndSave);

            IsConfigMode = true;
            IsPlayMode = false;
        }


        private async Task SaveToMongoDB(List<QuestionPack> questionPacks)
        {
            foreach (var pack in questionPacks)
            {
                Debug.WriteLine($"Saving pack: {pack.Name}, Questions count: {pack.Questions.Count}");
            }

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MarkusTobin");

            var mongoDBService = new MongoDBService(database);
            await mongoDBService.SaveToMongoDBService(questionPacks);

        }

        private async Task LoadFromMongoDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MarkusTobin");
            var mongoDBService = new MongoDBService(database);
            List<QuestionPack> questionPacks = await mongoDBService.LoadFromMongoDBService();
            foreach (var pack in questionPacks)
            {
                Packs.Add(new QuestionPackViewModel(pack));
            }
            Debug.WriteLine($"{Packs.Count} QuestionPacks loaded.");

            if (Packs.Any())
            {
                ActivePack = Packs.First();

            }
        }

        private void SelectPack(object selectedPackObj)
        {
            if (selectedPackObj is QuestionPackViewModel selectedPack)
            {
                ActivePack = selectedPack;
            }
        }

        public void SetActivePack(QuestionPack newPack)
        {
            ActivePack = new QuestionPackViewModel(newPack);
            RaisePropertyChanged(nameof(ActivePack));
            PlayerViewModel.StartNewQuiz(ActivePack.Questions.ToList());
        }

        public void CloseDialogWindow(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
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

        private void ToggleFullScreen(object parameter)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;

            if (mainWindow == null)
            {
                Debug.WriteLine("Error: MainWindow is not available.");
                return;
            }

            if (mainWindow.WindowState == WindowState.Normal)
            {
                mainWindow.WindowState = WindowState.Maximized;
                mainWindow.WindowStyle = WindowStyle.None;
            }
            else
            {
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }
        private async void ExitAndSave(object parameter)
        {
            var result = System.Windows.MessageBox.Show(
                "You will save all Questionpacks on exit. Do you want to continue?",
                "Confirm Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            RaisePropertyChanged(nameof(Packs));

            if (result == MessageBoxResult.Yes)
            {
                var questionPacks = Packs.Select(p => p.Model).ToList();
                await SaveToMongoDB(questionPacks);
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}