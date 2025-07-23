using System.Diagnostics;

namespace JapaneseLearningApp.Views.Hiragana;

public partial class HiraganaMenuPage : ContentPage
{
	public HiraganaMenuPage()
	{
		InitializeComponent();
		BindingContext = new ViewModels.Hiragana.HiraganaMenuViewModel();
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if(BindingContext is ViewModels.Hiragana.HiraganaMenuViewModel viewModel)
		{
			viewModel.RefreshStatisticsCommand.ExecuteAsync(null);
        }
		else
		{
			System.Diagnostics.Debug.WriteLine("BindingContext is not set correctly.");
        }
    }


	private async void OnBrowseCharactersTapped(object sender, TappedEventArgs e)
	{
        System.Diagnostics.Debug.WriteLine("Browse Characters Tapped");

        if (BindingContext is ViewModels.Hiragana.HiraganaMenuViewModel viewModel)
		{
			await viewModel.NavigateToBrowseCommand.ExecuteAsync(null);
		}
		else
		{
			System.Diagnostics.Debug.WriteLine("BindingContext is not set correctly.");
        }
    }

	private async void OnPracticeModeTapped(object sender, TappedEventArgs e)
	{
		if (BindingContext is ViewModels.Hiragana.HiraganaMenuViewModel viewModel)
		{
			await viewModel.NavigateToPracticeCommand.ExecuteAsync(null);
		}
		else
		{
			System.Diagnostics.Debug.WriteLine("BindingContext is not set correctly.");
        }
    }

	private async void OnQuizModeTapped(object sender, TappedEventArgs e)
	{
		if (BindingContext is ViewModels.Hiragana.HiraganaMenuViewModel viewModel)
		{
			await viewModel.NavigateToQuizCommand.ExecuteAsync(null);
		}
		else
		{
			System.Diagnostics.Debug.WriteLine("BindingContext is not set correctly.");
        }
    }
}