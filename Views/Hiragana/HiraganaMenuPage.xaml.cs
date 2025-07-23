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


	private async void OnBrowseCharactersTapped(object sender, EventArgs e)
	{
		if (BindingContext is ViewModels.Hiragana.HiraganaMenuViewModel viewModel)
		{
			await viewModel.NavigateToBrowseCommand.ExecuteAsync(null);
		}
		else
		{
			System.Diagnostics.Debug.WriteLine("BindingContext is not set correctly.");
        }
    }

	private async void OnPracticeModeTapped(object sender, EventArgs e)
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

	private async void OnQuizModeTapped(object sender, EventArgs e)
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