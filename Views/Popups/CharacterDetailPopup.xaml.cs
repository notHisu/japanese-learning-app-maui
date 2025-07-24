using CommunityToolkit.Maui.Views;
using JapaneseLearningApp.Models;

namespace JapaneseLearningApp.Views.Popups;

public partial class CharacterDetailPopup : Popup
{
    public HiraganaCharacter Character { get; }

    public CharacterDetailPopup(HiraganaCharacter character)
    {
        InitializeComponent();
        Character = character;
        BindingContext = this;
    }

    private void OnCloseButtonClicked(object sender, EventArgs e)
    {
        Close();
    }

    private void OnMarkAsLearnedClicked(object sender, EventArgs e)
    {
        // Toggle the learning state
        if (Character.Progress == LearningProgress.NotStudied)
        {
            Character.Progress = LearningProgress.InProgress;
        }
        else if (Character.Progress == LearningProgress.InProgress)
        {
            Character.Progress = LearningProgress.Mastered;
            Character.IsLearned = true;
        }
        else
        {
            // Reset if already mastered
            Character.Progress = LearningProgress.NotStudied;
            Character.IsLearned = false;
        }

        Character.LastStudied = DateTime.Now;
        
        // Return the updated character
        Close(Character);
    }

    private async void OnPlayPronunciationClicked(object sender, EventArgs e)
    {
        // TODO: Implement audio playback
        await DisplayAlert("Audio", $"Playing pronunciation for {Character.Character}", "OK");
    }

    private async void OnViewStrokeOrderClicked(object sender, EventArgs e)
    {
        // TODO: Implement stroke order animation
        await DisplayAlert("Stroke Order", $"Showing stroke order for {Character.Character}", "OK");
    }

    // Helper method for popup alerts
    private Task DisplayAlert(string title, string message, string cancel)
    {
        if (Handler?.MauiContext != null)
        {
            var page = Handler.MauiContext.Services.GetService<IServiceProvider>()?.GetService<Page>();
            return page?.DisplayAlert(title, message, cancel) ?? Task.CompletedTask;
        }
        return Task.CompletedTask;
    }
}