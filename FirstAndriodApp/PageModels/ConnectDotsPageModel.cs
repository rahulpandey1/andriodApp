using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Graphics;

namespace FirstAndriodApp.PageModels;

public partial class ConnectDotsPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<DotItem> _dots = new();

    [ObservableProperty]
    private string _statusText = "Connect the numbers in order!";

    [ObservableProperty]
    private bool _isCompleted;
    
    [ObservableProperty]
    private string _revealedImage;

    private int _nextIndex = 0;

    public ConnectDotsPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        var levels = await _levelService.LoadLevelsAsync("connectdots");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);
        
        if (level == null) return;

        RevealedImage = level.Items.FirstOrDefault()?.Image ?? "";
        
        Dots.Clear();
        for (int i = 0; i < level.Items.Count; i++)
        {
            var item = level.Items[i];
            var parts = item.CorrectMatch.Split(',');
            if (parts.Length == 2 && double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) && double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
            {
                Dots.Add(new DotItem { Id = i + 1, X = x, Y = y, IsConnected = false });
            }
        }
        
        _nextIndex = 0;
        IsCompleted = false;
        StatusText = "Start at 1!";
    }

    public void DotTouched(int id)
    {
        if (IsCompleted) return;

        // Sequence Check
        // If user touches 1, then we wait for 2.
        // If user touches Correct Next Dot
        
        // Actually this Logic should be: User drags from Last Connected to Next.
        // We handle Hit Testing in View, verify logic here.
    }
    
    public bool TryConnect(int fromId, int toId)
    {
        // Must connect in order
        if (fromId == _nextIndex + 1 && toId == _nextIndex + 2)
        {
             _audioService.PlaySound("pop.mp3");
            _nextIndex++;
            
            var dot = Dots.FirstOrDefault(d => d.Id == fromId);
            if(dot != null) dot.IsConnected = true;

            if (_nextIndex == Dots.Count - 1)
            {
                // Last connection made
                Dots.Last().IsConnected = true;
                IsCompleted = true;
                StatusText = "Picture Revealed!";
                _audioService.PlaySound("win.mp3");
                _gameManager.EndGame(true, 3);
            }
            return true;
        }
        return false;
    }

    [RelayCommand]
    public void Reset() => LoadLevel(1);
}

public partial class DotItem : ObservableObject
{
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    
    [ObservableProperty]
    private bool _isConnected;
}
