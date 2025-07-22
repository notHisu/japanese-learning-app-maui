using JapaneseLearningApp.ViewModels;

namespace JapaneseLearningApp.Views;

public partial class KanaPage : ContentPage
{
    public KanaPage()
    {
        InitializeComponent();
        BindingContext = new KanaViewModel();
    }
}
