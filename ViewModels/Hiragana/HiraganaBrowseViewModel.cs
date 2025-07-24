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

        public async Task InitializeAsync()
        {
            await LoadCharactersAsync();
        }

        [RelayCommand]
        private async Task LoadCharactersAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            try
            {
                _allCharacters = await _hiraganaService.GetAllCharactersAsync();
                ApplyFilter(SelectedFilter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading characters: {ex.Message}");
                // TODO: Show error message to user
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Filter(string filterType)
        {
            SelectedFilter = filterType;
            ApplyFilter(filterType);
        }

        private void ApplyFilter(string filterType)
        {
            var filtered = filterType.ToLowerInvariant() switch
            {
                "all" => _allCharacters,
                "dakuten" => _allCharacters.Where(c => 
                    c.Group == Models.Enums.CharacterGroup.Dakuten || 
                    c.Group == Models.Enums.CharacterGroup.Handakuten).ToList(),
                _ => _allCharacters.Where(c => 
                    c.Row.Equals(filterType, StringComparison.OrdinalIgnoreCase)).ToList()
            };

            FilteredCharacters.Clear();
            foreach (var character in filtered.OrderBy(c => c.Id))
            {
                FilteredCharacters.Add(character);
            }
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
                    ApplyFilter(SelectedFilter);
                    
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
                new() { Id = 1, Character = "あ", Romanji = "a", Row = "a-row", StrokeCount = 3, Progress = LearningProgress.Mastered },
                new() { Id = 2, Character = "か", Romanji = "ka", Row = "ka-row", StrokeCount = 3, Progress = LearningProgress.InProgress },
                new() { Id = 3, Character = "さ", Romanji = "sa", Row = "sa-row", StrokeCount = 3, Progress = LearningProgress.NotStudied }
            };
            
            _allCharacters = sampleCharacters;
            ApplyFilter("All");
        }
    }
}
