using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Labb3___GUI.ViewModel
{
    internal class CategoryViewModel : ViewModelBase
    {
        private ObservableCollection<string> _categories;
        private string _selectedCategory;

        public CategoryViewModel()
        {
            Task.Run(() => LoadCategories());
        }
        public CategoryViewModel(ObservableCollection<string> categories)
        {
            Categories = categories;
            Task.Run(() => LoadCategories());
        }

        public async Task LoadCategories()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MarkusTobin");
            var collection = database.GetCollection<BsonDocument>("Categories");

            var categoriesFromDB = await collection.Find(new BsonDocument()).ToListAsync();

            Categories = new ObservableCollection<string>();

            if (categoriesFromDB.Count == 0)
            {
                Categories = new ObservableCollection<string>
                {
                    "Science",
                    "Math",
                    "Other"
                };
            }
            else
            {
                foreach (var category in categoriesFromDB)
                {
                    Categories.Add(category["Name"].AsString);
                }
            }
        }

        public ObservableCollection<string> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                RaisePropertyChanged();
                if (_categories != null && _categories.Count > 0 && SelectedCategory == null) { SelectedCategory = _categories[0]; }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged();
            }
        }
    }
}