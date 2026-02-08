using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class MemoryFlipPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<MemoryCard> _cards = new();

    [ObservableProperty]
    private string _statusText = "Flip two cards to find a pair.";

    [ObservableProperty]
    private int _moves;

    [ObservableProperty]
    private int _matchedPairs;
    
    [ObservableProperty]
    private bool _isCompleted;

    private int _totalPairs;
    private int _currentLevelId = 1;

    MemoryCard? first;
    MemoryCard? second;
    bool isBusy;

    public MemoryFlipPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        
        LoadLevel(1);
    }
    
    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("memoryflip");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
             // Fallback or restart
             level = levels.FirstOrDefault(); 
             if (level == null) return;
        }
        
        // Prepare deck from level items
        Cards.Clear();
        var deck = level.Items
            .SelectMany(p => new[]
            {
                new MemoryCard { PairId = p.Id, Image = p.Image, IsFlipped = false, IsMatched = false },
                new MemoryCard { PairId = p.Id, Image = p.Image, IsFlipped = false, IsMatched = false },
            })
            .OrderBy(_ => Guid.NewGuid())
            .ToList();

        foreach (var c in deck)
            Cards.Add(c);

        _totalPairs = level.Items.Count;
        
        ResetState();
    }

    [RelayCommand]
    public void Reset()
    {
        LoadLevel(_currentLevelId);
    }

    private void ResetState()
    {
        first = null;
        second = null;
        isBusy = false;
        Moves = 0;
        MatchedPairs = 0;
        IsCompleted = false;
        StatusText = "Flip two cards to find a pair.";
    }

    public async Task FlipAsync(MemoryCard card)
    {
        if (isBusy || card.IsMatched || card.IsFlipped) return;

        _audioService.PlaySound("flip.mp3");
        
        card.IsFlipped = true;

        if (first == null)
        {
            first = card;
            return;
        }

        second = card;
        Moves++;

        if (first.PairId == second.PairId)
        {
            first.IsMatched = true;
            second.IsMatched = true;

            first = null;
            second = null;

            MatchedPairs++;
            _audioService.PlaySound("success.mp3");
            StatusText = "Match!";
            
            if (MatchedPairs == _totalPairs)
            {
                IsCompleted = true;
                StatusText = "All pairs matched!";
                _audioService.PlaySound("win.mp3");
                _gameManager.EndGame(true, 3);
            }
            return;
        }

        StatusText = "Not a match.";
        isBusy = true;
        await Task.Delay(1000);

        if (first != null) first.IsFlipped = false;
        if (second != null) second.IsFlipped = false;

        first = null;
        second = null;
        isBusy = false;
        StatusText = "Try again.";
    }
}
