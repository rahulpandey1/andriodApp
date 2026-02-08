using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class ColorMixingPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private string _statusText = "Mix two colors!";

    [ObservableProperty]
    private Color _targetColor;
    
    [ObservableProperty]
    private Color _mixedColor = Colors.White; // Start neutral

    [ObservableProperty]
    private Color _color1 = Colors.Transparent;
    
    [ObservableProperty]
    private Color _color2 = Colors.Transparent;

    public ObservableCollection<ColorItem> AvailableColors { get; } = new();

    private int _currentLevelId = 1;

    public ColorMixingPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;

        // Initialize Palette
        AvailableColors.Add(new ColorItem { Name = "Red", Color = Colors.Red });
        AvailableColors.Add(new ColorItem { Name = "Yellow", Color = Colors.Yellow });
        AvailableColors.Add(new ColorItem { Name = "Blue", Color = Colors.Blue });
        AvailableColors.Add(new ColorItem { Name = "White", Color = Colors.White });

        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("colormixing");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            StatusText = "Master Artist!";
            return;
        }

        string hex = level.Items[0].CorrectMatch;
        TargetColor = Color.FromArgb(hex);
        StatusText = $"Make {level.Difficulty}!";
        
        Color1 = Colors.Transparent;
        Color2 = Colors.Transparent;
        UpdateMix();
    }

    [RelayCommand]
    public void SelectColor(ColorItem item)
    {
        _audioService.PlaySound("blop.mp3");
        
        if (Color1 == Colors.Transparent)
        {
            Color1 = item.Color;
        }
        else if (Color2 == Colors.Transparent)
        {
            Color2 = item.Color;
        }
        else
        {
            // Reset and start new
            Color1 = item.Color;
            Color2 = Colors.Transparent;
        }
        
        UpdateMix();
    }

    private async void UpdateMix()
    {
        if (Color1 == Colors.Transparent) 
        {
            MixedColor = Colors.White;
            return;
        }
        
        if (Color2 == Colors.Transparent)
        {
            MixedColor = Color1;
            return;
        }

        // Mix Logic (Simplified RGB avg)
        // Or better, hardcoded lookup for standard color theory
        MixedColor = MixColors(Color1, Color2);

        // Check Win
        if (AreColorsSimilar(MixedColor, TargetColor))
        {
            _audioService.PlaySound("win.mp3");
            StatusText = "Perfect Match!";
            await Task.Delay(1500);
            LoadLevel(_currentLevelId + 1);
        }
    }

    private Color MixColors(Color c1, Color c2)
    {
        // Simple lookup
        if ((c1 == Colors.Red && c2 == Colors.Yellow) || (c1 == Colors.Yellow && c2 == Colors.Red)) return Color.FromArgb("#FFA500"); // Orange
        if ((c1 == Colors.Blue && c2 == Colors.Yellow) || (c1 == Colors.Yellow && c2 == Colors.Blue)) return Color.FromArgb("#008000"); // Green
        if ((c1 == Colors.Red && c2 == Colors.Blue) || (c1 == Colors.Blue && c2 == Colors.Red)) return Color.FromArgb("#800080"); // Purple
        if ((c1 == Colors.Red && c2 == Colors.White) || (c1 == Colors.White && c2 == Colors.Red)) return Colors.Pink; 
        
        // Fallback: Average
        return Color.FromRgba((c1.Red + c2.Red)/2, (c1.Green + c2.Green)/2, (c1.Blue + c2.Blue)/2, 1);
    }
    
    private bool AreColorsSimilar(Color c1, Color c2)
    {
        // Approximate
        return Math.Abs(c1.Red - c2.Red) < 0.1 && 
               Math.Abs(c1.Green - c2.Green) < 0.1 && 
               Math.Abs(c1.Blue - c2.Blue) < 0.1;
    }

    [RelayCommand]
    public void Reset() 
    {
        Color1 = Colors.Transparent;
        Color2 = Colors.Transparent;
        UpdateMix();
    }
}

public class ColorItem
{
    public string Name { get; set; }
    public Color Color { get; set; }
}
