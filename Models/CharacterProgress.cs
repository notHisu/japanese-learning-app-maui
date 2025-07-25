using System;

namespace JapaneseLearningApp.Models
{
    public class CharacterProgress
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public int CorrectAttempts { get; set; } = 0;
        public int TotalAttempts { get; set; } = 0;
        public DateTime? LastStudied { get; set; }
        public DateTime? LastCorrect { get; set; }
        public LearningProgress Progress { get; set; } = LearningProgress.NotStudied;
        public int ConsecutiveCorrect { get; set; } = 0;
        public int ConsecutiveIncorrect { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Computed properties
        public double AccuracyRate => TotalAttempts > 0 ? (double)CorrectAttempts / TotalAttempts * 100 : 0;
        public bool NeedsPractice => Progress != LearningProgress.Mastered ||
                                   (LastStudied.HasValue && LastStudied.Value.AddDays(7) < DateTime.UtcNow);
        public int DaysSinceLastStudied => LastStudied.HasValue ?
                                         (DateTime.UtcNow - LastStudied.Value).Days : int.MaxValue;
    }

    public class UserStatistics
    {
        public int TotalCharacters { get; set; }
        public int StudiedCharacters { get; set; }
        public int MasteredCharacters { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }
        public double OverallAccuracy { get; set; }
        public DateTime? LastStudyDate { get; set; }
        public int TotalPracticeSessions { get; set; }
        public TimeSpan TotalStudyTime { get; set; }

        // Computed properties
        public double ProgressPercentage => TotalCharacters > 0 ?
                                          (double)MasteredCharacters / TotalCharacters * 100 : 0;
        public int CharactersInProgress => StudiedCharacters - MasteredCharacters;
    }
}