using JapaneseLearningApp.ViewModels.Hiragana;

namespace JapaneseLearningApp.Views.Hiragana;

public partial class HiraganaPracticePage : ContentPage
{
    public HiraganaPracticePage()
    {
        InitializeComponent();
        BindingContext = new HiraganaPracticeViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HiraganaPracticeViewModel viewModel)
        {
            // Reset to setup view when page appears
            viewModel.ShowSetup = true;
            viewModel.IsPracticeActive = false;
        }
    }
}