using JapaneseLearningApp.Models;
using JapaneseLearningApp.Models.Enums;
using System.Reflection;
using System.Text.Json;

namespace JapaneseLearningApp.Services
{
    public class HiraganaService : IHiraganaService
    {
        private List<HiraganaCharacter> _cachedCharacters = new();
        private bool _isDataLoaded = false;
        private readonly SemaphoreSlim _loadingSemaphore = new(1, 1);

        public async void LoadHiraganaDataAsync()
        {
            await LoadDataIfNeededAsync();
        }

        public async Task<List<HiraganaCharacter>> GetAllCharactersAsync()
        {
            await LoadDataIfNeededAsync();
            return _cachedCharacters.ToList(); // Return a copy to prevent external modifications
        }

        public async Task<HiraganaCharacter> GetCharacterByIdAsync(int id)
        {
            await LoadDataIfNeededAsync();
            var character = _cachedCharacters.FirstOrDefault(c => c.Id == id);

            if (character == null)
            {
                throw new ArgumentException($"Character with ID {id} not found.", nameof(id));
            }

            return character;
        }

        public async Task<List<HiraganaCharacter>> GetCharactersByRowAsync(string row)
        {
            if (string.IsNullOrWhiteSpace(row))
            {
                throw new ArgumentException("Row cannot be null or empty.", nameof(row));
            }

            await LoadDataIfNeededAsync();
            return _cachedCharacters
                .Where(c => c.Row.Equals(row, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task UpdateCharacterProgressAsync(HiraganaCharacter character)
        {
            if (character == null)
                throw new ArgumentNullException(nameof(character));

            await LoadDataIfNeededAsync();
            
            var existingCharacter = _cachedCharacters.FirstOrDefault(c => c.Id == character.Id);
            if (existingCharacter != null)
            {
                existingCharacter.Progress = character.Progress;
                existingCharacter.IsLearned = character.IsLearned;
                existingCharacter.LastStudied = character.LastStudied;
                existingCharacter.CorrectAttempts = character.CorrectAttempts;
                existingCharacter.TotalAttempts = character.TotalAttempts;

                // TODO: Persist changes to local storage or database
                System.Diagnostics.Debug.WriteLine($"Updated progress for character {character.Character}: {character.Progress}");
            }
        }

        private async Task LoadDataIfNeededAsync()
        {
            if (_isDataLoaded)
                return;

            await _loadingSemaphore.WaitAsync();
            try
            {
                if (_isDataLoaded) // Double-check after acquiring the lock
                    return;

                await LoadHiraganaDataFromJsonAsync();
                _isDataLoaded = true;
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        private async Task LoadHiraganaDataFromJsonAsync()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = await FileSystem.OpenAppPackageFileAsync("hiragana_data.json");
                using var reader = new StreamReader(stream);
                var jsonContent = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var jsonData = JsonSerializer.Deserialize<HiraganaDataRoot>(jsonContent, options);

                if (jsonData?.Characters == null)
                {
                    throw new InvalidOperationException("Failed to load hiragana data: Invalid JSON structure.");
                }

                _cachedCharacters = jsonData.Characters
                    .Select(MapJsonToModel)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Successfully loaded {_cachedCharacters.Count} hiragana characters.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading hiragana data: {ex.Message}");
                throw new InvalidOperationException("Failed to load hiragana data from JSON file.", ex);
            }
        }

        private static HiraganaCharacter MapJsonToModel(HiraganaJsonModel jsonModel)
        {
            return new HiraganaCharacter
            {
                Id = jsonModel.Id,
                Character = jsonModel.Character ?? string.Empty,
                Group = ParseCharacterGroup(jsonModel.Group),
                Romanji = jsonModel.Romaji ?? string.Empty,
                Row = jsonModel.Row ?? string.Empty,
                StrokeCount = jsonModel.StrokeCount,
                Examples = jsonModel.Examples ?? new List<string>()
            };
        }

        private static CharacterGroup ParseCharacterGroup(string? group)
        {
            return group?.ToLowerInvariant() switch
            {
                "basic" => CharacterGroup.Basic,
                "dakuten" => CharacterGroup.Dakuten,
                "handakuten" => CharacterGroup.Handakuten,
                "combination" => CharacterGroup.Combination,
                _ => CharacterGroup.Basic
            };
        }

        // JSON models for deserialization
        private class HiraganaDataRoot
        {
            public List<HiraganaJsonModel> Characters { get; set; } = new();
        }

        private class HiraganaJsonModel
        {
            public int Id { get; set; }
            public string? Character { get; set; }
            public string? Romaji { get; set; }
            public string? Group { get; set; }
            public string? Row { get; set; }
            public int StrokeCount { get; set; }
            public List<string>? Examples { get; set; }
        }
    }
}