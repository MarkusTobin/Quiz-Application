using Labb3___GUI.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Labb3___GUI.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;
        private DispatcherTimer timer;
        private string _testData;

        public string TestData { 
            get => _testData;
            private set
            {
                _testData = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand UpdateButtonCommand { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            TestData = "Start value: ";

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //  timer.Start();

            UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);

        }

        private bool CanUpdateButton(object? arg)
        {
            return TestData.Length < 15;
        }

        private void UpdateButton(object obj)
        {
            TestData += "x";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TestData += "x";
        }
    }
}
