﻿using Labb3___GUI.Command;
using Labb3___GUI.Dialogs;
using Labb3___GUI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using MongoDB.Driver;

namespace Labb3___GUI.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        public DelegateCommand AddButtonCommand { get; }
        public DelegateCommand RemoveButtonCommand { get; }

        public DelegateCommand EditPackCommand { get; }

        public ConfigurationViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            VisibilityMode = Visibility.Hidden;
            IsEnabled = true;

            AddButtonCommand = new DelegateCommand(AddButton);
            RemoveButtonCommand = new DelegateCommand(RemoveButton, CanRemoveQuestion);
            EditPackCommand = new DelegateCommand(EditPack, CanEditPack);

            ActivePack?.Questions.Add(new Question("Question abc", "a", "b", "c", "d"));
            ActiveQuestion = ActivePack?.Questions.FirstOrDefault();
        }
        public void EditPack(object? parameter)
        {
            if (ActivePack != null)
            {

                var dialog = new PackOptionsDialog();
                var dialogViewModel = new QuestionPack
                {
                    Name = ActivePack.Name,
                    Difficulty = ActivePack.Difficulty,
                    TimeLimitInSeconds = ActivePack.TimeLimitInSeconds
                };

                dialog.DataContext = dialogViewModel;

                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    ActivePack.Name = dialogViewModel.Name;
                    ActivePack.Difficulty = dialogViewModel.Difficulty;
                    ActivePack.TimeLimitInSeconds = dialogViewModel.TimeLimitInSeconds;
                }
            }
        }
        private void AddButton(object? parameter)
        {
            VisibilityMode = Visibility.Visible;

            var newQuestion = new Question("Default", "1", "2", "3", "4");
            ActivePack?.Questions.Add(newQuestion);
            ActiveQuestion = newQuestion;
            RemoveButtonCommand.RaiseCanExecuteChanged();

        }
        private void RemoveButton(object? parameter)
        {
            ActivePack?.Questions.Remove(ActiveQuestion);
            ActiveQuestion = ActivePack?.Questions.LastOrDefault();
            RemoveButtonCommand.RaiseCanExecuteChanged();
        }

        private ObservableCollection<Question> _questions;
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel.ActivePack; }

        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                RaisePropertyChanged(nameof(Questions));
                Debug.WriteLine("Questions updated:", _questions.Count);
            }
        }

        private Question? _activeQuestion;
        public Question? ActiveQuestion
        {
            get => _activeQuestion; set
            {
                _activeQuestion = value; RaisePropertyChanged();
                VisibilityMode = (_activeQuestion != null) ? Visibility.Visible : Visibility.Hidden;
                RemoveButtonCommand.RaiseCanExecuteChanged();
            }
        }

        private Visibility _visibility;
        public Visibility VisibilityMode { get => _visibility; set { _visibility = value; RaisePropertyChanged(); } }
        private bool RemoveActiveButton(object? arg)
        {
            return IsEnabled = true;
        }
        private bool CanRemoveQuestion(object? parameter) => ActiveQuestion != null;

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set { _isEnabled = value; RaisePropertyChanged(); ; } }
        private bool CanEditPack(object arg)
        {
            return ActivePack != null;
        }
    }
}