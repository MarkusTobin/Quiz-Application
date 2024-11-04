using Labb3___GUI.Command;
using Labb3___GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3___GUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
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
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }
        public ConfigurationViewModel ConfigurationViewModel { get;}
        public PlayerViewModel PlayerViewModel { get; }

        public DelegateCommand StartQuizCommand { get; }
        public DelegateCommand SetConfigModeCommand { get; }
        public DelegateCommand SetPlayModeCommand { get; }
        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);
            PlayerViewModel = new PlayerViewModel(this);
            Packs = new ObservableCollection<QuestionPackViewModel>();
            ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));

            StartQuizCommand = new DelegateCommand(StartQuiz);
            SetConfigModeCommand = new DelegateCommand(_ => SetConfigMode());
            SetPlayModeCommand = new DelegateCommand(_ => SetPlayMode());

            IsConfigMode = true; // might not me needed
            IsPlayMode = false;  // might not me needed
        }

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
