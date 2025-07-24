using Microsoft.Maui.Controls;

namespace JapaneseLearningApp.Views.Hiragana;

public partial class HiraganaBrowsePage : ContentPage
{
    public HiraganaBrowsePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewModels.Hiragana.HiraganaBrowseViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    // Remove the tap animation since we're using popup
    // The popup handles its own interaction feedback
}