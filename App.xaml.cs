using JapaneseLearningApp.Views.Hiragana;

namespace JapaneseLearningApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute("hiragana/menu", typeof(HiraganaMenuPage));
            Routing.RegisterRoute("hiragana/browse", typeof(HiraganaBrowsePage));
            Routing.RegisterRoute("hiragana/practice", typeof(HiraganaPracticePage));
            Routing.RegisterRoute("hiragana/quiz", typeof(HiraganaQuizPage));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}