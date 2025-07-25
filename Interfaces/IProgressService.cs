using JapaneseLearningApp.Models;

namespace JapaneseLearningApp.Services
{
    public interface IProgressService
    {
        Task<CharacterProgress> GetProgressAsync(int characterId);
        Task UpdateProgressAsync(int characterId, bool isCorrect);
        Task<List<CharacterProgress>> GetAllProgressAsync();
        Task SaveProgressAsync(CharacterProgress progress);
        Task<UserStatistics> GetUserStatisticsAsync();
        Task ResetProgressAsync(int characterId);
        Task<List<CharacterProgress>> GetCharactersNeedingPracticeAsync();
    }
}