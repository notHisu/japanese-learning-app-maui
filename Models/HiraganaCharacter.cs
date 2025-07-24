using JapaneseLearningApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseLearningApp.Models
{
    public class HiraganaCharacter
    {
        public int Id { get; set; }
        public string Character { get; set; } = string.Empty;
        public string Romanji { get; set; } = string.Empty;
        public CharacterGroup Group { get; set; }
        public string Row { get; set; } = string.Empty;
        public int StrokeCount { get; set; }
        public string? StrokeOrder { get; set; } = string.Empty;
        public string? AudioFile { get; set; } = string.Empty;
        public bool IsLearned { get; set; } = false;
        public List<string> Examples { get; set; } = new List<string>();

        // Additional progress tracking properties
        public LearningProgress Progress { get; set; } = LearningProgress.NotStudied;
        public int CorrectAttempts { get; set; } = 0;
        public int TotalAttempts { get; set; } = 0;
        public DateTime? LastStudied { get; set; }
        public double AccuracyRate => TotalAttempts > 0 ? (double)CorrectAttempts / TotalAttempts * 100 : 0;

        // Computed properties for UI
        public bool IsNotStudied => Progress == LearningProgress.NotStudied;
        public bool IsInProgress => Progress == LearningProgress.InProgress;
        public bool IsMastered => Progress == LearningProgress.Mastered;
        
        public string ProgressColor => Progress switch
        {
            LearningProgress.NotStudied => "#95A5A6", // Gray
            LearningProgress.InProgress => "#3498DB", // Blue
            LearningProgress.Mastered => "#27AE60",   // Green
            _ => "#95A5A6"
        };

        public string ProgressBackgroundColor => Progress switch
        {
            LearningProgress.NotStudied => "#ECEFF1", // Light Gray
            LearningProgress.InProgress => "#E3F2FD", // Light Blue
            LearningProgress.Mastered => "#E8F5E8",   // Light Green
            _ => "#ECEFF1"
        };

        public string ProgressIcon => Progress switch
        {
            LearningProgress.NotStudied => "📖", // Book
            LearningProgress.InProgress => "🔄", // Arrows
            LearningProgress.Mastered => "✅",   // Check mark
            _ => "📖"
        };
    }

    public enum LearningProgress
    {
        NotStudied,
        InProgress,
        Mastered
    }
}
