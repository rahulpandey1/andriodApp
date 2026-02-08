using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Graphics;

namespace FirstAndriodApp.PageModels;

public partial class SpotDifferencePageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private string _statusText = "Find the difference!";

    [ObservableProperty]
    private string _baseImage = "background_park.png"; // Placeholder background

    [ObservableProperty]
    private ObservableCollection<DifferenceItem> _differences = new();

    private int _foundCount;
    private int _targetCount;
    private int _currentLevelId = 1;

    public SpotDifferencePageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("spotthedifference");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            StatusText = "You found them all!";
            return;
        }

        // Use a generic background or one defined in level
        // For now, hardcode or reuse an existing large image
        BaseImage = "background_park.png"; // We don't have this, let's use puzzle_puppy
        if(BaseImage == "background_park.png") BaseImage = "puzzle_puppy.png"; 

        Differences.Clear();
        foreach (var item in level.Items)
        {
             var parts = item.CorrectMatch.Split(',');
             if (parts.Length == 2 && double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) && double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
             {
                 Differences.Add(new DifferenceItem 
                 { 
                     Id = item.Id, 
                     Image = item.Image, 
                     X = x, 
                     Y = y,
                     IsFound = false 
                 });
             }
        }

        _targetCount = Differences.Count;
        _foundCount = 0;
        StatusText = $"Find {_targetCount} differences!";
    }

    [RelayCommand]
    public async Task DifferenceTapped(DifferenceItem item)
    {
        if (item.IsFound) return;

        item.IsFound = true; // Use checkmark or hide?
        // Let's hide it or show a ring to indicate found?
        // Actually, if it's an "added" item, maybe we remove it?
        // Or we just mark it.
        // Let's fade it out or animate.
        
        _audioService.PlaySound("success.mp3");
        _foundCount++;

        if (_foundCount >= _targetCount)
        {
            StatusText = "Level Complete!";
            await Task.Delay(1000);
            
             // Check if next level exists
            var levels = await _levelService.LoadLevelsAsync("spotthedifference");
            if (levels.Any(l => l.LevelId == _currentLevelId + 1))
            {
                 LoadLevel(_currentLevelId + 1);
            }
            else
            {
                _gameManager.EndGame(true, 3);
            }
        }
    }

    [RelayCommand]
    public void Reset() => LoadLevel(1);
}

public partial class DifferenceItem : ObservableObject
{
    public string Id { get; set; }
    public string Image { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Opacity))]
    private bool _isFound;

    public double Opacity => IsFound ? 0.3 : 1.0; 
    public bool IsVisible => !IsFound; // Or keep visible but faded
}
