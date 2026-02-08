using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class ShadowMatchPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<MatchItem> _leftItems = new();

    [ObservableProperty]
    private ObservableCollection<MatchItem> _rightItems = new();

    [ObservableProperty]
    private string _statusText = "Match the object to its shadow!";

    [ObservableProperty]
    private int _matchedCount;
    
    [ObservableProperty]
    private bool _isCompleted;

    private List<MatchItem> _currentLevelItems = new();
    private int _currentLevelId = 1;

    public ShadowMatchPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("shadowmatch");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            StatusText = "All Levels Completed!";
            return;
        }

        _currentLevelItems = level.Items.Select(i => new MatchItem 
        { 
            Id = i.Id, 
            ObjectImage = i.Image, 
            ShadowImage = i.CorrectMatch 
        }).ToList();

        Reset();
    }

    [RelayCommand]
    public void Reset()
    {
        LeftItems.Clear();
        RightItems.Clear();

        if (_currentLevelItems.Count == 0) return;

        var leftOrder = _currentLevelItems.OrderBy(_ => Guid.NewGuid()).ToList();
        var rightOrder = _currentLevelItems.OrderBy(_ => Guid.NewGuid()).ToList();

        foreach (var i in leftOrder)
            LeftItems.Add(new MatchItem { Id = i.Id, ObjectImage = i.ObjectImage, ShadowImage = i.ShadowImage, IsMatched = false });

        foreach (var i in rightOrder)
            RightItems.Add(new MatchItem { Id = i.Id, ObjectImage = i.ObjectImage, ShadowImage = i.ShadowImage, IsMatched = false });

        MatchedCount = 0;
        IsCompleted = false;
        StatusText = $"Level {_currentLevelId}: Match Shadows!";
    }

    public async Task<bool> CheckMatch(string droppedId, string targetId)
    {
        if (droppedId == targetId)
        {
            _audioService.PlaySound("success.mp3");
            ApplyMatch(droppedId);
            
            if (IsCompleted)
            {
                _audioService.PlaySound("win.mp3");
                _gameManager.EndGame(true, 3);
                StatusText = "Awesome! Next Level...";
                await Task.Delay(2000);
                LoadLevel(_currentLevelId + 1);
            }
            return true;
        }
        else
        {
            _audioService.PlaySound("error.mp3");
            StatusText = "Oops! Try again.";
            return false;
        }
    }

    private void ApplyMatch(string id)
    {
        var leftIndex = LeftItems.ToList().FindIndex(x => x.Id == id);
        var rightIndex = RightItems.ToList().FindIndex(x => x.Id == id);

        if (leftIndex < 0 || rightIndex < 0) return;

        LeftItems[leftIndex] = new MatchItem
        {
            Id = $"blank_{id}",
            ObjectImage = string.Empty, 
            IsMatched = true
        };

        var right = RightItems[rightIndex];
        RightItems[rightIndex] = new MatchItem
        {
            Id = right.Id,
            ObjectImage = right.ShadowImage.Replace("_shadow", "_color"), // Hack to show color
            ShadowImage = _currentLevelItems.First(x => x.Id == id).ObjectImage, 
            IsMatched = true
        };

        MatchedCount++;
        IsCompleted = MatchedCount == _currentLevelItems.Count;
    }
}
