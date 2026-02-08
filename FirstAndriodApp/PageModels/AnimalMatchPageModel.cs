using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class AnimalMatchPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<MatchItem> _animals = new();

    [ObservableProperty]
    private string _statusText = "Tap the speaker to listen!";

    [ObservableProperty]
    private bool _isPlayingSound;

    private List<MatchItem> _currentLevelItems = new();
    private MatchItem? _currentTarget;
    private int _score;
    private int _targetScore = 5;
    private int _currentLevelId = 1;

    public AnimalMatchPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("animalmatch");
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
            ShadowImage = i.CorrectMatch, // Storing Sound Filename here
            Label = i.Id 
        }).ToList();

        _score = 0;
        Animals.Clear();
        foreach(var item in _currentLevelItems) Animals.Add(item);

        NextRound();
    }

    private void NextRound()
    {
        if (_score >= _targetScore)
        {
            _audioService.PlaySound("win.mp3");
            _gameManager.EndGame(true, 3);
            StatusText = "You verified all animals!";
            return;
        }

        // Pick random target
        var rnd = new Random();
        _currentTarget = _currentLevelItems[rnd.Next(_currentLevelItems.Count)];
        
        StatusText = "Listen to the sound...";
        PlayTargetSound();
    }

    [RelayCommand]
    public void PlayTargetSound()
    {
        if (_currentTarget == null) return;
        
        IsPlayingSound = true;
        // Assuming AudioService handles concurrent play or we just fire and forget
        _audioService.PlaySound(_currentTarget.ShadowImage); 
        
        // Simple delay to reset visual state if we tracked duration, but for now just toggle back shortly
        Task.Delay(1000).ContinueWith(t => IsPlayingSound = false);
    }

    [RelayCommand]
    public async Task CheckAnswer(MatchItem selected)
    {
        if (_currentTarget == null) return;

        if (selected.Id == _currentTarget.Id)
        {
            _audioService.PlaySound("success.mp3");
            StatusText = $"Correct! It's a {selected.Label}!";
            await Task.Delay(1000);
            _score++;
            NextRound();
        }
        else
        {
            _audioService.PlaySound("error.mp3");
            StatusText = "Try again!";
            // Shake visual handled by view checking bool return? 
            // Or just allow rapid fire guessing.
        }
    }

    [RelayCommand]
    public void Reset() => LoadLevel(_currentLevelId);
}
