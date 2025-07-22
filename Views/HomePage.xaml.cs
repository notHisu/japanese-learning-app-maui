using Microsoft.Maui.Controls;

namespace JapaneseLearningApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnVocabularyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///VocabularyPage");
    }

    private async void OnKanjiClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///KanjiPage");
    }

    private async void OnQuizClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///QuizPage");
    }

    private async void OnKanaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///KanaPage");
    }
}
