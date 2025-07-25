using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JapaneseLearningApp.Models;
using JapaneseLearningApp.Services;
using System.Collections.ObjectModel;

namespace JapaneseLearningApp.ViewModels.Hiragana
{
    public partial class HiraganaPracticeViewModel : ObservableObject
    {
        private readonly IHiraganaService _hiraganaService;
        private readonly IProgressService _progressService;
        private List<HiraganaCharacter> _characterPool = new();
        private PracticeSession _currentSession = new();

        // Current practice state
        [ObservableProperty]
        private HiraganaCharacter currentCharacter;

        [ObservableProperty]
        private string currentQuestion = string.Empty;

        [ObservableProperty]
        private string userAnswer = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> multipleChoiceOptions = new();

        [ObservableProperty]
        private string selectedAnswer = string.Empty;

        // Practice mode
        [ObservableProperty]
        private PracticeMode practiceMode = PracticeMode.Recognition;

        // Progress tracking
        [ObservableProperty]
        private int currentQuestionNumber = 1;

        [ObservableProperty]
        private int totalQuestions = 10;

        [ObservableProperty]
        private int correctCount = 0;

        [ObservableProperty]
        private int streakCount = 0;

        [ObservableProperty]
        private double progressPercentage = 0;

        // UI States
        [ObservableProperty]
        private bool isCheckingAnswer = false;

        [ObservableProperty]
        private bool showResult = false;

        [ObservableProperty]
        private bool isCorrect = false;

        [ObservableProperty]
        private string feedbackMessage = string.Empty;

        [ObservableProperty]
        private bool isPracticeActive = false;

        [ObservableProperty]
        private bool showSetup = true;

        [ObservableProperty]
        private bool canCheckAnswer = false;

        // Constructor
        public HiraganaPracticeViewModel(IHiraganaService hiraganaService, IProgressService progressService)
        {
            _hiraganaService = hiraganaService;
            _progressService = progressService;
        }

        public HiraganaPracticeViewModel()
        {
            // For XAML preview
            _hiraganaService = new HiraganaService();
            _progressService = new ProgressService();
        }

        // Commands
        [RelayCommand]
        private async Task StartPracticeAsync()
        {
            try
            {
                await LoadCharacterPoolAsync();
                _currentSession = new PracticeSession
                {
                    StartTime = DateTime.Now,
                    Mode = PracticeMode,
                    TotalQuestions = TotalQuestions
                };

                ShowSetup = false;
                IsPracticeActive = true;
                await LoadNextQuestionAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting practice: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task CheckAnswerAsync()
        {
            if (IsCheckingAnswer) return;

            IsCheckingAnswer = true;
            ShowResult = true;

            // Validate answer based on mode
            bool correct = ValidateAnswer();
            IsCorrect = correct;

            if (correct)
            {
                CorrectCount++;
                StreakCount++;
                FeedbackMessage = "Correct! 🎉";
            }
            else
            {
                StreakCount = 0;
                FeedbackMessage = $"Incorrect. The answer is: {GetCorrectAnswer()}";
            }

            // Update progress
            UpdateProgress();
            await SaveAnswerResultAsync(correct);

            // Auto-advance after delay
            await Task.Delay(2000);
            await NextQuestionAsync();
        }

        [RelayCommand]
        private async Task NextQuestionAsync()
        {
            if (CurrentQuestionNumber >= TotalQuestions)
            {
                await EndPracticeAsync();
                return;
            }

            CurrentQuestionNumber++;
            UpdateProgressPercentage();
            await LoadNextQuestionAsync();
            ResetQuestionState();
        }

        [RelayCommand]
        private void SelectPracticeMode(string mode)
        {
            if (Enum.TryParse<PracticeMode>(mode, out var parsedMode))
            {
                PracticeMode = parsedMode;
            }
        }

        [RelayCommand]
        private async Task EndPracticeAsync()
        {
            _currentSession.EndTime = DateTime.Now;
            _currentSession.CorrectAnswers = CorrectCount;

            IsPracticeActive = false;
            ShowSetup = true;

            // Save session and navigate back
            await SaveSessionAsync();
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void SelectMultipleChoiceOption(string option)
        {
            SelectedAnswer = option;
            UserAnswer = option;
            CanCheckAnswer = !string.IsNullOrEmpty(option);
        }

        // Private methods
        private async Task LoadCharacterPoolAsync()
        {
            var allCharacters = await _hiraganaService.GetAllCharactersAsync();

            // Prioritize characters that need practice
            _characterPool = allCharacters
                .OrderBy(c => c.Progress)
                .ThenBy(c => Guid.NewGuid()) // Random within same progress level
                .Take(TotalQuestions * 2) // Get more than needed for variety
                .ToList();
        }

        private async Task LoadNextQuestionAsync()
        {
            if (!_characterPool.Any()) return;

            // Select character (avoid recent repeats)
            CurrentCharacter = SelectNextCharacter();

            // Generate question based on mode
            await GenerateQuestionAsync();
        }

        private HiraganaCharacter SelectNextCharacter()
        {
            var recentCharacters = _currentSession.Results
                .TakeLast(3)
                .Select(r => r.CharacterId)
                .ToHashSet();

            var availableCharacters = _characterPool
                .Where(c => !recentCharacters.Contains(c.Id))
                .ToList();

            if (!availableCharacters.Any())
                availableCharacters = _characterPool;

            var random = new Random();
            return availableCharacters[random.Next(availableCharacters.Count)];
        }

        private async Task GenerateQuestionAsync()
        {
            switch (PracticeMode)
            {
                case PracticeMode.Recognition:
                    await GenerateRecognitionQuestionAsync();
                    break;
                case PracticeMode.Production:
                    await GenerateProductionQuestionAsync();
                    break;
                case PracticeMode.MultipleChoice:
                    await GenerateMultipleChoiceQuestionAsync();
                    break;
            }
        }

        private async Task GenerateRecognitionQuestionAsync()
        {
            CurrentQuestion = $"What is the romaji for {CurrentCharacter.Character}?";
            MultipleChoiceOptions.Clear();
            UserAnswer = string.Empty;
            SelectedAnswer = string.Empty;
            CanCheckAnswer = false;
        }

        private async Task GenerateProductionQuestionAsync()
        {
            CurrentQuestion = $"Which hiragana represents '{CurrentCharacter.Romanji}'?";

            var allCharacters = await _hiraganaService.GetAllCharactersAsync();
            var options = GenerateHiraganaOptions(CurrentCharacter, allCharacters);

            MultipleChoiceOptions.Clear();
            foreach (var option in options)
            {
                MultipleChoiceOptions.Add(option);
            }

            UserAnswer = string.Empty;
            SelectedAnswer = string.Empty;
            CanCheckAnswer = false;
        }

        private async Task GenerateMultipleChoiceQuestionAsync()
        {
            var questionTypes = new[] { "recognition", "production" };
            var questionType = questionTypes[new Random().Next(questionTypes.Length)];

            if (questionType == "recognition")
            {
                CurrentQuestion = $"What is the romaji for {CurrentCharacter.Character}?";
                var allCharacters = await _hiraganaService.GetAllCharactersAsync();
                var options = GenerateRomajiOptions(CurrentCharacter, allCharacters);

                MultipleChoiceOptions.Clear();
                foreach (var option in options)
                {
                    MultipleChoiceOptions.Add(option);
                }
            }
            else
            {
                await GenerateProductionQuestionAsync();
            }

            UserAnswer = string.Empty;
            SelectedAnswer = string.Empty;
            CanCheckAnswer = false;
        }

        private List<string> GenerateRomajiOptions(HiraganaCharacter correct, List<HiraganaCharacter> allCharacters)
        {
            var options = new List<string> { correct.Romanji };

            // Add 3 similar romaji options
            var similar = allCharacters
                .Where(c => c.Id != correct.Id)
                .OrderBy(c => Guid.NewGuid())
                .Take(3)
                .Select(c => c.Romanji)
                .ToList();

            options.AddRange(similar);
            return options.OrderBy(o => Guid.NewGuid()).ToList();
        }

        private List<string> GenerateHiraganaOptions(HiraganaCharacter correct, List<HiraganaCharacter> allCharacters)
        {
            var options = new List<string> { correct.Character };

            // Add 3 different hiragana characters
            var others = allCharacters
                .Where(c => c.Id != correct.Id)
                .OrderBy(c => Guid.NewGuid())
                .Take(3)
                .Select(c => c.Character)
                .ToList();

            options.AddRange(others);
            return options.OrderBy(o => Guid.NewGuid()).ToList();
        }

        private bool ValidateAnswer()
        {
            return PracticeMode switch
            {
                PracticeMode.Recognition => UserAnswer.Trim().ToLower() == CurrentCharacter.Romanji.ToLower(),
                PracticeMode.Production => UserAnswer == CurrentCharacter.Character,
                PracticeMode.MultipleChoice => UserAnswer == GetCorrectAnswer(),
                _ => false
            };
        }

        private string GetCorrectAnswer()
        {
            return PracticeMode switch
            {
                PracticeMode.Recognition => CurrentCharacter.Romanji,
                PracticeMode.Production => CurrentCharacter.Character,
                PracticeMode.MultipleChoice => CurrentQuestion.Contains("romaji") ? CurrentCharacter.Romanji : CurrentCharacter.Character,
                _ => string.Empty
            };
        }

        private void UpdateProgress()
        {
            UpdateProgressPercentage();
        }

        private void UpdateProgressPercentage()
        {
            ProgressPercentage = (double)CurrentQuestionNumber / TotalQuestions * 100;
        }

        private async Task SaveAnswerResultAsync(bool isCorrect)
        {
            var result = new PracticeResult
            {
                CharacterId = CurrentCharacter.Id,
                Question = CurrentQuestion,
                UserAnswer = UserAnswer,
                CorrectAnswer = GetCorrectAnswer(),
                IsCorrect = isCorrect,
                Timestamp = DateTime.Now
            };

            _currentSession.Results.Add(result);
            _currentSession.PracticedCharacterIds.Add(CurrentCharacter.Id);

            // Update character progress in service
            await _progressService?.UpdateProgressAsync(CurrentCharacter.Id, isCorrect);
        }

        private async Task SaveSessionAsync()
        {
            // TODO: Implement session saving to database
            System.Diagnostics.Debug.WriteLine($"Practice session completed: {_currentSession.AccuracyRate:F1}% accuracy");
        }

        private void ResetQuestionState()
        {
            ShowResult = false;
            IsCheckingAnswer = false;
            UserAnswer = string.Empty;
            SelectedAnswer = string.Empty;
            FeedbackMessage = string.Empty;
            CanCheckAnswer = false;
        }

        // Property change handlers
        partial void OnUserAnswerChanged(string value)
        {
            if (PracticeMode == PracticeMode.Recognition)
            {
                CanCheckAnswer = !string.IsNullOrWhiteSpace(value);
            }
        }
    }
}