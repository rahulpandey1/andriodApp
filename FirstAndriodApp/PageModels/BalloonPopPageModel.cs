using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;

namespace FirstAndriodApp.PageModels;

public partial class BalloonPopPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;
    private IDispatcherTimer _gameTimer;

    [ObservableProperty]
    private string _statusText = "Pop items in order!";

    [ObservableProperty]
    private ObservableCollection<BalloonItem> _balloons = new();

    private List<LevelItem> _levelItems;
    private int _nextNumber;
    private bool _isPlaying;
    private Random _random = new();

    public double GameWidth { get; set; } = 350;
    public double GameHeight { get; set; } = 600;

    public BalloonPopPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
    }

    public void StartGame(IDispatcherTimer timer)
    {
        _gameTimer = timer;
        _gameTimer.Interval = TimeSpan.FromMilliseconds(50);
        _gameTimer.Tick += GameLoop;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        var levels = await _levelService.LoadLevelsAsync("balloonpop");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);
        
        if (level == null) return;

        _levelItems = level.Items;
        _nextNumber = 1;
        Balloons.Clear();
        _isPlaying = true;
        
        // Pre-spawn all items but position them off-screen or staggered?
        // Let's spawn them periodically or all at bottom.
        // For counting 1-5, let's just spawn all 5 immediately at random X and bottom Y.
        
        foreach (var item in _levelItems)
        {
            Balloons.Add(new BalloonItem
            {
                Id = item.Id,
                Image = item.Image,
                Number = int.Parse(item.CorrectMatch),
                X = _random.Next(20, (int)(GameWidth - 80)),
                Y = GameHeight + _random.Next(0, 400), // Start below
                Speed = _random.Next(2, 6)
            });
        }
        
        _gameTimer.Start();
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        StatusText = $"Pop Number {_nextNumber}!";
    }

    private void GameLoop(object sender, EventArgs e)
    {
        if (!_isPlaying) return;

        // Move Balloons Up
        for (int i = 0; i < Balloons.Count; i++)
        {
            var b = Balloons[i];
            b.Y -= b.Speed;

            // Reset if goes off top
            if (b.Y < -100)
            {
                b.Y = GameHeight + 100;
                b.X = _random.Next(20, (int)(GameWidth - 80));
            }
        }
    }

    [RelayCommand]
    public void PopBalloon(BalloonItem balloon)
    {
        if (!_isPlaying) return;

        if (balloon.Number == _nextNumber)
        {
            _audioService.PlaySound("pop.mp3");
            Balloons.Remove(balloon);
            _nextNumber++;

            if (Balloons.Count == 0)
            {
                EndGame(true);
            }
            else
            {
                UpdateStatus();
            }
        }
        else
        {
            _audioService.PlaySound("bump.mp3");
            StatusText = $"No! Find {_nextNumber}!";
        }
    }

    private void EndGame(bool win)
    {
        _isPlaying = false;
        _gameTimer.Stop();
        StatusText = "Great Counting!";
        if(win) _gameManager.EndGame(true, 3);
    }

    public void Stop() => _gameTimer?.Stop();
}

public partial class BalloonItem : ObservableObject
{
    public string Id { get; set; }
    public string Image { get; set; }
    public int Number { get; set; }
    public double Speed { get; set; }

    [ObservableProperty]
    private double _x;
    
    [ObservableProperty]
    private double _y;
}
