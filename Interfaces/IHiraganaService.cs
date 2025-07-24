using JapaneseLearningApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseLearningApp.Services
{
    public interface IHiraganaService
    {
        void LoadHiraganaDataAsync();
        Task<List<HiraganaCharacter>> GetAllCharactersAsync();
        Task<List<HiraganaCharacter>> GetCharactersByRowAsync(string row);
        Task<HiraganaCharacter> GetCharacterByIdAsync(int id);
        Task UpdateCharacterProgressAsync(HiraganaCharacter character);
    }
}
