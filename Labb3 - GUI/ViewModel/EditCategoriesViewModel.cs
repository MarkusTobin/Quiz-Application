using Labb3___GUI.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3___GUI.ViewModel
{
    internal class EditCategoriesViewModel : ViewModelBase
    {
        public ObservableCollection<string> Categories { get; }
        private string _newCategory;
        public string NewCategory
        {
            get => _newCategory;
            set
            {
                _newCategory = value;
                RaisePropertyChanged();
                AddCategoryCommand.RaiseCanExecuteChanged();
            }
        }
        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged();
                RemoveCategoryCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AddCategoryCommand { set; get; }
        public DelegateCommand RemoveCategoryCommand { get; }

        public EditCategoriesViewModel(ObservableCollection<string> categories)
        {
            Categories = categories;
            AddCategoryCommand = new DelegateCommand(_ => AddCategory(), _ => CanAddCategory());
            RemoveCategoryCommand = new DelegateCommand(RemoveCategory, CanRemoveCategory);

        }
        private void AddCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewCategory) && !Categories.Contains(NewCategory))
            {
                //rensa lite här kanske
                Categories.Add(NewCategory);
                SelectedCategory = NewCategory;
                RaisePropertyChanged(nameof(NewCategory));
                AddCategoryCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(Categories));
                RaisePropertyChanged(nameof(SelectedCategory));
            }
        }
        private bool CanAddCategory() => !string.IsNullOrWhiteSpace(NewCategory) && !Categories.Contains(NewCategory);

        private void RemoveCategory(object category)
        {
            if (category is string categoryToRemove)
            {
                Categories.Remove(categoryToRemove);
            }
        }
        private bool CanRemoveCategory(object category) => category is string categoryToRemove && Categories.Contains(categoryToRemove);
    }
}
