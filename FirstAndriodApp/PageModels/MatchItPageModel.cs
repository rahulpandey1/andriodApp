using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class MatchItPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<MatchItem> _leftItems = new();

    [ObservableProperty]
    private ObservableCollection<MatchItem> _rightItems = new();

    [ObservableProperty]
    private string _statusText = "Drag an object onto its hollow picture.";

    [ObservableProperty]
    private int _matchedCount;
    
    [ObservableProperty]
    private bool _isCompleted;

    private List<MatchItem> _currentLevelItems = new();
    private int _currentLevelId = 1;

    public MatchItPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("matchit");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            StatusText = "Game Over! You finished all levels.";
            return;
        }

        _currentLevelItems = level.Items.Select(i => new MatchItem 
        { 
            Id = i.Id, 
            ObjectImage = i.Image, 
            ShadowImage = i.CorrectMatch,
            Label = i.Id // Optional
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
        {
            LeftItems.Add(new MatchItem
            {
                Id = i.Id,
                ObjectImage = i.ObjectImage,
                ShadowImage = i.ShadowImage,
                IsMatched = false
            });
        }

        foreach (var i in rightOrder)
        {
            RightItems.Add(new MatchItem
            {
                Id = i.Id,
                ObjectImage = i.ObjectImage, // Initially show object too? No, ShadowImage
                ShadowImage = i.ShadowImage,
                IsMatched = false
            });
        }

        MatchedCount = 0;
        IsCompleted = false;
        StatusText = $"Level {_currentLevelId}: Drag to match!";
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
                _gameManager.EndGame(true, 3); // 3 stars for now
                StatusText = "Great Job! Loading next level...";
                await Task.Delay(2000);
                LoadLevel(_currentLevelId + 1);
            }
            return true;
        }
        else
        {
            _audioService.PlaySound("error.mp3");
            StatusText = "Try again!";
            return false;
        }
    }

    private void ApplyMatch(string id)
    {
        var leftIndex = LeftItems.ToList().FindIndex(x => x.Id == id);
        var rightIndex = RightItems.ToList().FindIndex(x => x.Id == id);

        if (leftIndex < 0 || rightIndex < 0) return;

        // Hide left item
        LeftItems[leftIndex] = new MatchItem
        {
            Id = $"blank_{id}",
            ObjectImage = string.Empty, // Hide
            IsMatched = true
        };

        // Complete right item matching
        var right = RightItems[rightIndex];
        RightItems[rightIndex] = new MatchItem
        {
            Id = right.Id,
            ObjectImage = right.ShadowImage.Replace("_outline", "_color"), // Hacky but effectively reveals color
            // Or better: use the ObjectImage from original source?
            // currentLevelItems.First(x => x.Id == id).ObjectImage
            ShadowImage = _currentLevelItems.First(x => x.Id == id).ObjectImage, 
            IsMatched = true
        };

        MatchedCount++;
        IsCompleted = MatchedCount == _currentLevelItems.Count;
    }
}
