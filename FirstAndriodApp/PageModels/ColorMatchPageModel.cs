using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class ColorMatchPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<MatchItem> _items = new();

    [ObservableProperty]
    private string _instructionText = "Find all Red items!";

    [ObservableProperty]
    private string _statusText = "Tap the items.";

    private List<MatchItem> _currentLevelItems = new();
    private int _currentLevelId = 1;
    private int _targetCount;
    private int _foundCount;

    public ColorMatchPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("colormatch");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            InstructionText = "You Win!";
            StatusText = "All levels completed.";
            return;
        }

        InstructionText = level.Difficulty; // "Find Red"

        _currentLevelItems.Clear();
        _targetCount = 0;

        foreach (var item in level.Items)
        {
            bool isCorrect = item.CorrectMatch == "true";
            if (isCorrect) _targetCount++;

            _currentLevelItems.Add(new MatchItem
            {
                Id = item.Id,
                ObjectImage = item.Image,
                Label = isCorrect ? "true" : "false", // Storing logic in Label property as a hack or create new Model? MatchItem reuse is fine.
                IsMatched = false
            });
        }

        Reset();
    }

    [RelayCommand]
    public void Reset()
    {
        Items.Clear();
        foreach (var item in _currentLevelItems.OrderBy(_ => Guid.NewGuid()))
        {
            // Reset state
            Items.Add(new MatchItem 
            { 
                Id = item.Id, 
                ObjectImage = item.ObjectImage, 
                Label = item.Label, 
                IsMatched = false 
            });
        }
        _foundCount = 0;
        StatusText = "Tap the correct items!";
    }

    [RelayCommand]
    public async Task TapItem(MatchItem item)
    {
        if (item.IsMatched) return;

        bool isCorrect = item.Label == "true";

        if (isCorrect)
        {
            _audioService.PlaySound("pop.mp3");
            item.IsMatched = true; // Updates UI to show checkmark/dim
            _foundCount++;
            
            // To force UI update if ObservableObject doesn't trigger on property change of Item unless Item implements INotifyPropertyChanged. 
            // MatchItem does NOT implement INotifyPropertyChanged in my assumption (it's a simple class?).
            // I need to check MatchItem.cs. 
            // If it doesn't, I need to replace the item in the collection to trigger update.
            var index = Items.IndexOf(item);
            if (index >= 0)
            {
                Items[index] = new MatchItem 
                { 
                    Id = item.Id, 
                    ObjectImage = item.ObjectImage, 
                    Label = item.Label, 
                    IsMatched = true 
                };
            }

            if (_foundCount == _targetCount)
            {
                _audioService.PlaySound("win.mp3");
                _gameManager.EndGame(true, 3);
                StatusText = "Perfect! Level Complete.";
                await Task.Delay(1500);
                LoadLevel(_currentLevelId + 1);
            }
        }
        else
        {
            _audioService.PlaySound("error.mp3");
            StatusText = "Wrong color! Try again.";
            // Shake or wiggle effect?
        }
    }
}
