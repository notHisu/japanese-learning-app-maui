using JapaneseLearningApp.Views.Hiragana;

namespace JapaneseLearningApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("hiragana/menu", typeof(HiraganaMenuPage));
            Routing.RegisterRoute("hiragana/browse", typeof(HiraganaBrowsePage));
            Routing.RegisterRoute("hiragana/practice", typeof(HiraganaPracticePage));
            Routing.RegisterRoute("hiragana/quiz", typeof(HiraganaQuizPage));
        }
    }
}
