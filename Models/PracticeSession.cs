using JapaneseLearningApp.Models.Enums; // Add this line
using System;
using System.Collections.Generic;

namespace JapaneseLearningApp.Models
{
    public class PracticeSession
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public PracticeMode Mode { get; set; } // This will now use the enum from Enums folder
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public List<int> PracticedCharacterIds { get; set; } = new();
        public List<PracticeResult> Results { get; set; } = new();
        public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;
        public double AccuracyRate => TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;
    }

    public class PracticeResult
    {
        public int CharacterId { get; set; }
        public string Question { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public DateTime Timestamp { get; set; }
    }
}