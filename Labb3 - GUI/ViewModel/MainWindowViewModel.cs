using Labb3___GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3___GUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }
        public ConfigurationViewModel ConfigurationViewModel { get;}
        public PlayerViewModel PlayerViewModel { get; }
        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);
            PlayerViewModel = new PlayerViewModel(this);
            ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
        }

        private QuestionPackViewModel? _activePack;
        public QuestionPackViewModel? ActivePack
		{
			get => _activePack;
			set 
			{ 
				_activePack = value;
                RaisePropertyChanged();
                ConfigurationViewModel.RaisePropertyChanged("ActivePack");
            }
		}

	}
}
