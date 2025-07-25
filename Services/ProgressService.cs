using JapaneseLearningApp.Models;
using System.Text.Json;

namespace JapaneseLearningApp.Services
{
    public class ProgressService : IProgressService
    {
        private readonly string _progressFilePath;
        private readonly string _statisticsFilePath;
        private List<CharacterProgress> _progressCache = new();
        private UserStatistics _statisticsCache = new();
        private bool _isInitialized = false;

        public ProgressService()
        {
            var appDataPath = FileSystem.AppDataDirectory;
            _progressFilePath = Path.Combine(appDataPath, "character_progress.json");
            _statisticsFilePath = Path.Combine(appDataPath, "user_statistics.json");
        }

        private async Task InitializeAsync()
        {
            if (_isInitialized) return;

            await LoadProgressFromFileAsync();
            await LoadStatisticsFromFileAsync();
            _isInitialized = true;
        }

        public async Task<CharacterProgress> GetProgressAsync(int characterId)
        {
            await InitializeAsync();

            var progress = _progressCache.FirstOrDefault(p => p.CharacterId == characterId);
            if (progress == null)
            {
                // Create new progress entry
                progress = new CharacterProgress
                {
                    CharacterId = characterId,
                    Progress = LearningProgress.NotStudied
                };
                _progressCache.Add(progress);
                await SaveProgressToFileAsync();
            }

            return progress;
        }

        public async Task UpdateProgressAsync(int characterId, bool isCorrect)
        {
            await InitializeAsync();

            var progress = await GetProgressAsync(characterId);

            // Update attempts
            progress.TotalAttempts++;
            if (isCorrect)
            {
                progress.CorrectAttempts++;
                progress.ConsecutiveCorrect++;
                progress.ConsecutiveIncorrect = 0;
                progress.LastCorrect = DateTime.UtcNow;
            }
            else
            {
                progress.ConsecutiveIncorrect++;
                progress.ConsecutiveCorrect = 0;
            }

            progress.LastStudied = DateTime.UtcNow;
            progress.UpdatedAt = DateTime.UtcNow;

            // Update learning progress based on performance
            UpdateLearningProgress(progress);

            await SaveProgressToFileAsync();
            await UpdateUserStatisticsAsync();
        }

        public async Task<List<CharacterProgress>> GetAllProgressAsync()
        {
            await InitializeAsync();
            return _progressCache.ToList();
        }

        public async Task SaveProgressAsync(CharacterProgress progress)
        {
            await InitializeAsync();

            var existingProgress = _progressCache.FirstOrDefault(p => p.CharacterId == progress.CharacterId);
            if (existingProgress != null)
            {
                _progressCache.Remove(existingProgress);
            }

            _progressCache.Add(progress);
            await SaveProgressToFileAsync();
        }

        public async Task<UserStatistics> GetUserStatisticsAsync()
        {
            await InitializeAsync();
            await UpdateUserStatisticsAsync();
            return _statisticsCache;
        }

        public async Task ResetProgressAsync(int characterId)
        {
            await InitializeAsync();

            var progress = _progressCache.FirstOrDefault(p => p.CharacterId == characterId);
            if (progress != null)
            {
                _progressCache.Remove(progress);
                await SaveProgressToFileAsync();
                await UpdateUserStatisticsAsync();
            }
        }

        public async Task<List<CharacterProgress>> GetCharactersNeedingPracticeAsync()
        {
            await InitializeAsync();

            return _progressCache
                .Where(p => p.NeedsPractice)
                .OrderBy(p => p.LastStudied ?? DateTime.MinValue)
                .ToList();
        }

        // Private helper methods
        private void UpdateLearningProgress(CharacterProgress progress)
        {
            var accuracy = progress.AccuracyRate;
            var totalAttempts = progress.TotalAttempts;
            var consecutiveCorrect = progress.ConsecutiveCorrect;

            // Determine learning progress based on performance
            if (consecutiveCorrect >= 5 && accuracy >= 90 && totalAttempts >= 5)
            {
                progress.Progress = LearningProgress.Mastered;
            }
            else if (totalAttempts >= 3 && accuracy >= 60)
            {
                progress.Progress = LearningProgress.InProgress;
            }
            else if (totalAttempts > 0)
            {
                progress.Progress = LearningProgress.InProgress;
            }
            else
            {
                progress.Progress = LearningProgress.NotStudied;
            }
        }

        private async Task UpdateUserStatisticsAsync()
        {
            var allProgress = _progressCache;

            _statisticsCache.TotalCharacters = 46; // Total hiragana characters
            _statisticsCache.StudiedCharacters = allProgress.Count(p => p.Progress != LearningProgress.NotStudied);
            _statisticsCache.MasteredCharacters = allProgress.Count(p => p.Progress == LearningProgress.Mastered);
            _statisticsCache.OverallAccuracy = allProgress.Any() ?
                allProgress.Average(p => p.AccuracyRate) : 0;
            _statisticsCache.LastStudyDate = allProgress
                .Where(p => p.LastStudied.HasValue)
                .Max(p => p.LastStudied);

            // Calculate current streak (simplified)
            _statisticsCache.CurrentStreak = CalculateCurrentStreak(allProgress);

            await SaveStatisticsToFileAsync();
        }

        private int CalculateCurrentStreak(List<CharacterProgress> allProgress)
        {
            // Simplified streak calculation
            // In a real app, you'd track daily study sessions
            var recentStudies = allProgress
                .Where(p => p.LastStudied.HasValue && p.LastStudied.Value.Date == DateTime.UtcNow.Date)
                .Count();

            return recentStudies > 0 ? Math.Min(recentStudies, 20) : 0;
        }

        private async Task LoadProgressFromFileAsync()
        {
            try
            {
                if (File.Exists(_progressFilePath))
                {
                    var json = await File.ReadAllTextAsync(_progressFilePath);
                    var progress = JsonSerializer.Deserialize<List<CharacterProgress>>(json);
                    _progressCache = progress ?? new List<CharacterProgress>();
                }
                else
                {
                    _progressCache = new List<CharacterProgress>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading progress: {ex.Message}");
                _progressCache = new List<CharacterProgress>();
            }
        }

        private async Task SaveProgressToFileAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_progressCache, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_progressFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving progress: {ex.Message}");
            }
        }

        private async Task LoadStatisticsFromFileAsync()
        {
            try
            {
                if (File.Exists(_statisticsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_statisticsFilePath);
                    var stats = JsonSerializer.Deserialize<UserStatistics>(json);
                    _statisticsCache = stats ?? new UserStatistics();
                }
                else
                {
                    _statisticsCache = new UserStatistics();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
                _statisticsCache = new UserStatistics();
            }
        }

        private async Task SaveStatisticsToFileAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_statisticsCache, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_statisticsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving statistics: {ex.Message}");
            }
        }
    }
}