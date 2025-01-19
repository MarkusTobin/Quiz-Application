using Labb3___GUI.Model;
using Labb3___GUI.ViewModel;
using System.Windows;

namespace Labb3___GUI.Dialogs
{

    public partial class CreateNewPackDialog : Window
    {
        public CreateNewPackDialog()
        {
            InitializeComponent();

        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}