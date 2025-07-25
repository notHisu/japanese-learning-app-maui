namespace JapaneseLearningApp.Models.Enums
{
    /// <summary>
    /// Different types of practice modes
    /// </summary>
    public enum PracticeMode
    {
        /// <summary>
        /// Show hiragana character, user types romaji
        /// </summary>
        Recognition,

        /// <summary>
        /// Show romaji, user selects/types hiragana
        /// </summary>
        Production,

        /// <summary>
        /// Multiple choice questions
        /// </summary>
        MultipleChoice,

        /// <summary>
        /// User traces character stroke order
        /// </summary>
        StrokeOrder,

        /// <summary>
        /// Timed flashcard review
        /// </summary>
        FlashCards,

        /// <summary>
        /// Audio-based practice
        /// </summary>
        AudioRecognition,

        /// <summary>
        /// Mix of different practice types
        /// </summary>
        Mixed
    }

    /// <summary>
    /// Performance rating for practice sessions
    /// </summary>
    public enum SessionPerformance
    {
        Poor,
        NeedsImprovement,
        Fair,
        Good,
        Excellent
    }

    /// <summary>
    /// Difficulty levels for practice
    /// </summary>
    public enum DifficultyLevel
    {
        /// <summary>
        /// Basic characters, more time, hints available
        /// </summary>
        Easy,

        /// <summary>
        /// Standard practice with moderate time limits
        /// </summary>
        Medium,

        /// <summary>
        /// Advanced characters, shorter time limits
        /// </summary>
        Hard,

        /// <summary>
        /// Combination of all difficulty levels
        /// </summary>
        Mixed,

        /// <summary>
        /// Adaptive difficulty based on performance
        /// </summary>
        Adaptive
    }
}