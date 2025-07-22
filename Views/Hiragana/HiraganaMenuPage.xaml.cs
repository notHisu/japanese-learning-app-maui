namespace JapaneseLearningApp.Views.Hiragana;

public partial class HiraganaMenuPage : ContentPage
{
	public HiraganaMenuPage()
	{
		InitializeComponent();
	}

	private async void OnBrowseCharactersTapped(object sender, EventArgs e)
	{
		// Navigate to browse characters page
		// await Shell.Current.GoToAsync("//browse-hiragana");
	}

	private async void OnPracticeModeTapped(object sender, EventArgs e)
	{
		// Navigate to practice mode page
		// await Shell.Current.GoToAsync("//practice-hiragana");
	}

	private async void OnQuizModeTapped(object sender, EventArgs e)
	{
		// Navigate to quiz mode page
		// await Shell.Current.GoToAsync("//quiz-hiragana");
	}
	// ...existing code...
}