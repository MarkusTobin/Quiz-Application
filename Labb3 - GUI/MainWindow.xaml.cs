using Labb3___GUI.ViewModel;
using System.Windows;


namespace Labb3___GUI
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}