using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class ShapeSorterPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<MatchItem> _shapes = new();

    [ObservableProperty]
    private string _statusText = "Drag shapes to the correct bin!";

    [ObservableProperty]
    private int _sortedCount;
    
    private List<MatchItem> _currentLevelItems = new();
    private int _currentLevelId = 1;

    public ShapeSorterPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("shapesorter");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            StatusText = "All Levels Completed!";
            return;
        }

        _shapes.Clear();
        _currentLevelItems.Clear();

        foreach (var item in level.Items)
        {
             var sortItem = new MatchItem 
             { 
                 Id = item.Id, 
                 ObjectImage = item.Image, 
                 Label = item.CorrectMatch, // Uses Label to store Bin Target (Circle, Square, Triangle)
                 IsMatched = false 
             };
             _shapes.Add(sortItem);
             _currentLevelItems.Add(sortItem);
        }

        SortedCount = 0;
        StatusText = $"Level {_currentLevelId}: Sort the shapes!";
    }

    [RelayCommand]
    public void Reset() => LoadLevel(_currentLevelId);

    public async Task<bool> CheckSort(string shapeId, string binId)
    {
        var item = Shapes.FirstOrDefault(x => x.Id == shapeId);
        if (item == null) return false;

        if (item.Label == binId) // Check if Bin Target matches dropped bin
        {
            _audioService.PlaySound("pop.mp3");
            
            // Remove item from list or mark matched
            item.IsMatched = true; // View should hide it
            Shapes.Remove(item); // Remove from view
            
            SortedCount++;
            
            if (Shapes.Count == 0)
            {
                _audioService.PlaySound("win.mp3");
                _gameManager.EndGame(true, 3);
                StatusText = "Great Job!";
                await Task.Delay(2000);
                LoadLevel(_currentLevelId + 1);
            }
            return true;
        }
        else
        {
            _audioService.PlaySound("error.mp3");
            StatusText = "Wrong bin!";
            return false;
        }
    }
}
