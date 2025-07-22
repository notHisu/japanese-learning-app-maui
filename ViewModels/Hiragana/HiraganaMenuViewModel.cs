using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseLearningApp.ViewModels.Hiragana
{
    public partial class HiraganaMenuViewModel : ObservableObject
    {
        [ObservableProperty]
        private int totalCharacters = 46;

        [ObservableProperty]
        private int learnedCharacters;

        [ObservableProperty]
        private int currentStreak;

        [ObservableProperty]
        private double accuracyRate;

        [ObservableProperty]
        private bool isLoading;

        [RelayCommand]
        private async Task NavigateToBrowseAsync()
        {

        }

        [RelayCommand]
        private async Task NavigateToQuizAsync()
        {
            // Logic to navigate to the quiz page
        }

        [RelayCommand]
        private async Task NavigateToPracticeAsync()
        {
            // Logic to navigate to the practice page
        }
    }
}
