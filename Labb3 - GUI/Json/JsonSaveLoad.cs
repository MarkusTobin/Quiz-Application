using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Labb3___GUI.Model;
using Labb3___GUI.ViewModel;

namespace Labb3___GUI.Json
{
    internal class JsonSaveLoad
    {
        public static async Task SavePacksToJson(MainWindowViewModel mainWindowViewModel)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Labb3Quiz","questionpacks.json");
                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string content = JsonSerializer.Serialize(mainWindowViewModel.Packs);
                await File.WriteAllTextAsync(filePath, content);
                MessageBox.Show($"Saved!", "Packs are", MessageBoxButton.OK);

               
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"Error saving QuestionPacks: {ex.Message}");
            }
        }     

        public static async Task LoadPacksFromJson(MainWindowViewModel mainWindowViewModel)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Labb3Quiz", "questionpacks.json");
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    var loadedQuestionPacks = JsonSerializer.Deserialize<List<QuestionPack>>(json);

                    if (loadedQuestionPacks != null)
                    {
                        mainWindowViewModel.Packs = new ObservableCollection<QuestionPackViewModel>(loadedQuestionPacks.Select(pack => new QuestionPackViewModel(pack))
);
                        mainWindowViewModel.ActivePack = mainWindowViewModel.Packs.FirstOrDefault();
                    }
                    else
                    {
                        mainWindowViewModel.Packs.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading packs: {ex.Message}", "Something went wrong :(", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Error saving QuestionPacks: {ex.Message}");
            }

        }

        
    }
}
