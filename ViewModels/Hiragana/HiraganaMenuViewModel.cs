using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace JapaneseLearningApp.ViewModels.Hiragana;

public partial class HiraganaMenuViewModel : ObservableObject
{
    [ObservableProperty]
    private int totalCharacters = 46;

    [ObservableProperty]
    private int learnedCharacters = 12;

    [ObservableProperty]
    private int currentStreak = 7;

    [ObservableProperty]
    private double accuracyRate = 85.0;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string progressPercentage = "26%";

    [ObservableProperty]
    private string progressText = "12/46 Characters Mastered";

    public HiraganaMenuViewModel()
    {
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        // Calculate progress percentage
        var percentage = (double)LearnedCharacters / TotalCharacters * 100;
        ProgressPercentage = $"{percentage:F0}%";
        ProgressText = $"{LearnedCharacters}/{TotalCharacters} Characters Mastered";
    }

    [RelayCommand]
    private async Task NavigateToBrowseAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("hiragana/browse");
        }
        catch (System.Exception ex)
        {
            // Handle navigation error
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task NavigateToPracticeAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("hiragana/practice");
        }
        catch (System.Exception ex)
        {
            // Handle navigation error
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task NavigateToQuizAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("hiragana/quiz");
        }
        catch (System.Exception ex)
        {
            // Handle navigation error
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task RefreshStatisticsAsync()
    {
        IsLoading = true;

        try
        {
            // Simulate loading delay
            await Task.Delay(1000);

            // TODO: Load real statistics from service
            LoadStatistics();
        }
        finally
        {
            IsLoading = false;
        }
    }
}