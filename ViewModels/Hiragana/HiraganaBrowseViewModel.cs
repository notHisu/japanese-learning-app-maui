using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JapaneseLearningApp.Models;
using JapaneseLearningApp.Services;
using JapaneseLearningApp.Views.Popups;
using System.Collections.ObjectModel;

namespace JapaneseLearningApp.ViewModels.Hiragana
{
    public partial class HiraganaBrowseViewModel : ObservableObject
    {
        private readonly IHiraganaService _hiraganaService;
        private List<HiraganaCharacter> _allCharacters = new();

        [ObservableProperty]
        private ObservableCollection<HiraganaCharacter> filteredCharacters = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string selectedFilter = "All";

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isSearchActive;

        [ObservableProperty]
        private bool isFilterTransitioning = false;

        [ObservableProperty]
        private bool showLoadingSkeleton = true;

        public HiraganaBrowseViewModel(IHiraganaService hiraganaService)
        {
            _hiraganaService = hiraganaService;
        }

        public HiraganaBrowseViewModel()
        {
            // Parameterless constructor for XAML preview
            _hiraganaService = new HiraganaService();
            
            // Initialize with sample data for design time
            if (Microsoft.Maui.Controls.DesignMode.IsDesignModeEnabled)
            {
                LoadSampleData();
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            IsSearchActive = !string.IsNullOrWhiteSpace(value);
            ApplyFilters();
        }

        public async Task InitializeAsync()
        {
            await LoadCharactersAsync();
        }

        [RelayCommand]
        private async Task LoadCharactersAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            ShowLoadingSkeleton = true;
            
            try
            {
                // Simulate loading delay for skeleton effect
                await Task.Delay(800);
                
                _allCharacters = await _hiraganaService.GetAllCharactersAsync();
                ApplyFilters();
                
                // Hide skeleton after data loads
                ShowLoadingSkeleton = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading characters: {ex.Message}");
                ShowLoadingSkeleton = false;
                // TODO: Show error message to user
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Filter(string filterType)
        {
            if (IsFilterTransitioning) return;

            IsFilterTransitioning = true;
            SelectedFilter = filterType;
            
            // Add smooth transition delay
            await Task.Delay(150);
            
            ApplyFilters();
            IsFilterTransitioning = false;
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        [RelayCommand]
        private void Search(string searchQuery)
        {
            SearchText = searchQuery ?? string.Empty;
        }

        private void ApplyFilters()
        {
            var characters = _allCharacters.AsEnumerable();

            // Apply category filter first
            characters = ApplyCategoryFilter(characters, SelectedFilter);

            // Apply search filter if search is active
            if (IsSearchActive)
            {
                characters = ApplySearchFilter(characters, SearchText);
            }

            // Update the filtered collection
            FilteredCharacters.Clear();
            foreach (var character in characters.OrderBy(c => c.Id))
            {
                FilteredCharacters.Add(character);
            }
        }

        private IEnumerable<HiraganaCharacter> ApplyCategoryFilter(IEnumerable<HiraganaCharacter> characters, string filterType)
        {
            return filterType.ToLowerInvariant() switch
            {
                "all" => characters,
                "dakuten" => characters.Where(c => 
                    c.Group == Models.Enums.CharacterGroup.Dakuten || 
                    c.Group == Models.Enums.CharacterGroup.Handakuten),
                _ => characters.Where(c => 
                    c.Row.Equals(filterType, StringComparison.OrdinalIgnoreCase))
            };
        }

        private IEnumerable<HiraganaCharacter> ApplySearchFilter(IEnumerable<HiraganaCharacter> characters, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return characters;

            var query = searchQuery.Trim().ToLowerInvariant();

            return characters.Where(character =>
                // Search by character
                character.Character.ToLowerInvariant().Contains(query) ||
                
                // Search by romaji
                character.Romanji.ToLowerInvariant().Contains(query) ||
                
                // Search by meaning (examples)
                character.Examples.Any(example => 
                    example.ToLowerInvariant().Contains(query)) ||
                
                // Search by row name
                character.Row.ToLowerInvariant().Contains(query) ||
                
                // Search by character group
                character.Group.ToString().ToLowerInvariant().Contains(query)
            );
        }

        private void ApplyFilter(string filterType)
        {
            // This method is kept for backward compatibility but now calls ApplyFilters()
            SelectedFilter = filterType;
            ApplyFilters();
        }

        [RelayCommand]
        private async Task CharacterSelected(HiraganaCharacter character)
        {
            if (character == null) return;

            try
            {
                // Show character detail popup
                var popup = new CharacterDetailPopup(character);
                
                var result = await Application.Current?.MainPage?.ShowPopupAsync(popup);
                
                // If result contains updated character, refresh the list
                if (result is HiraganaCharacter updatedCharacter)
                {
                    // Find and update the character in our collections
                    var originalChar = _allCharacters.FirstOrDefault(c => c.Id == updatedCharacter.Id);
                    if (originalChar != null)
                    {
                        originalChar.Progress = updatedCharacter.Progress;
                        originalChar.IsLearned = updatedCharacter.IsLearned;
                        originalChar.LastStudied = updatedCharacter.LastStudied;
                    }

                    // Refresh the filtered view
                    ApplyFilters();
                    
                    // TODO: Save changes via service
                    await _hiraganaService.UpdateCharacterProgressAsync(updatedCharacter);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing character popup: {ex.Message}");
            }
        }

        private void LoadSampleData()
        {
            // Sample data for design mode
            var sampleCharacters = new List<HiraganaCharacter>
            {
                new() { Id = 1, Character = "あ", Romanji = "a", Row = "a-row", StrokeCount = 3, Progress = LearningProgress.Mastered, Examples = new List<string> { "愛 (ai) - love", "朝 (asa) - morning" } },
                new() { Id = 2, Character = "か", Romanji = "ka", Row = "ka-row", StrokeCount = 3, Progress = LearningProgress.InProgress, Examples = new List<string> { "猫 (neko) - cat", "顔 (kao) - face" } },
                new() { Id = 3, Character = "さ", Romanji = "sa", Row = "sa-row", StrokeCount = 3, Progress = LearningProgress.NotStudied, Examples = new List<string> { "桜 (sakura) - cherry blossom", "魚 (sakana) - fish" } }
            };
            
            _allCharacters = sampleCharacters;
            ShowLoadingSkeleton = false;
            ApplyFilters();
        }
    }
}
