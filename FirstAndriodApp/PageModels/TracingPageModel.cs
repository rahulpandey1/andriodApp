using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class TracingPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private string _currentLetterImage = string.Empty;

    [ObservableProperty]
    private string _statusText = "Trace the letter!";

    private List<MatchItem> _letters = new();
    private int _currentIndex = 0;

    public TracingPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        var levels = await _levelService.LoadLevelsAsync("alphabettracing");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);
        
        if (level == null)
        {
            StatusText = "All Letters Traced!";
            return;
        }

        _letters = level.Items.Select(i => new MatchItem { Id = i.Id, ObjectImage = i.Image }).ToList();
        _currentIndex = 0;
        LoadLetter();
    }

    private void LoadLetter()
    {
        if (_currentIndex < _letters.Count)
        {
            CurrentLetterImage = _letters[_currentIndex].ObjectImage;
            StatusText = $"Trace Letter {_letters[_currentIndex].Id}";
        }
        else
        {
             _audioService.PlaySound("win.mp3");
            _gameManager.EndGame(true, 3);
            StatusText = "Great Job! You traced them all.";
        }
    }

    [RelayCommand]
    public async Task CompleteLetter()
    {
        _audioService.PlaySound("sparkle.mp3");
        StatusText = "Beautiful!";
        await Task.Delay(1000);
        _currentIndex++;
        LoadLetter();
        // Signal View to clear canvas? We need an event or messaging.
        // For simplicity, View subscribes or we rely on PropertyChange to clear if logic exists there.
        // Or cleaner: MessagingCenter/WeakReferenceMessenger to clear.
        WeakReferenceMessenger.Default.Send(new ClearCanvasMessage());
    }

    [RelayCommand]
    public void Reset() 
    {
        _currentIndex = 0;
        LoadLetter();
        WeakReferenceMessenger.Default.Send(new ClearCanvasMessage());
    }
}

public class ClearCanvasMessage {}
