using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;

namespace FirstAndriodApp.PageModels;

public partial class FruitCatchPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;
    private IDispatcherTimer _gameTimer;

    [ObservableProperty]
    private string _statusText = "Catch the fruit!";

    [ObservableProperty]
    private double _basketX = 150;
    
    [ObservableProperty]
    private int _score = 0;

    [ObservableProperty]
    private double _basketY = 500;
    
    public double BasketWidth { get; set; } = 80;
    public double BasketHeight { get; set; } = 60;
    public double GameWidth { get; set; } = 350; // Approx
    public double GameHeight { get; set; } = 600; // Approx

    [ObservableProperty]
    private ObservableCollection<FallingItem> _items = new();

    private List<LevelItem> _levelItems;
    private int _targetScore;
    private bool _isPlaying;
    private Random _random = new();
    private int _tickCount;

    public FruitCatchPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
    }

    public void StartGame(IDispatcherTimer timer)
    {
        _gameTimer = timer;
        _gameTimer.Interval = TimeSpan.FromMilliseconds(30); // ~30 FPS
        _gameTimer.Tick += GameLoop;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        var levels = await _levelService.LoadLevelsAsync("fruitcatch");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);
        
        if (level == null) return;

        _levelItems = level.Items;
        _targetScore = level.TargetScore;
        Score = 0;
        Items.Clear();
        _isPlaying = true;
        _gameTimer.Start();
        StatusText = $"Catch {_targetScore} fruits!";
    }

    private void GameLoop(object sender, EventArgs e)
    {
        if (!_isPlaying) return;
        _tickCount++;

        // Spawn
        if (_tickCount % 40 == 0) // Every ~1.2 sec
        {
            SpawnItem();
        }

        // Update Positions
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var item = Items[i];
            item.Y += 5; // Fall Speed

            // Check Collision
            if (CheckCollision(item))
            {
                // Caught
                HandleCatch(item);
                Items.RemoveAt(i);
            }
            else if (item.Y > GameHeight)
            {
                // Missed
                Items.RemoveAt(i);
            }
        }
    }

    private void SpawnItem()
    {
        var config = _levelItems[_random.Next(_levelItems.Count)];
        Items.Add(new FallingItem
        {
            Image = config.Image,
            Points = int.Parse(config.CorrectMatch),
            X = _random.Next(0, (int)(GameWidth - 50)),
            Y = -50
        });
    }

    private bool CheckCollision(FallingItem item)
    {
        // Simple Rect intersect
        // Basket: BasketX, BasketY, BasketWidth, BasketHeight
        // Item: item.X, item.Y, 50, 50
        
        bool xOverlap = _basketX < item.X + 40 && _basketX + BasketWidth > item.X + 10;
        bool yOverlap = BasketY < item.Y + 40 && BasketY + BasketHeight > item.Y + 10;
        
        return xOverlap && yOverlap;
    }

    private void HandleCatch(FallingItem item)
    {
        if (item.Points < 0)
        {
             _audioService.PlaySound("crash.mp3");
             StatusText = "Ouch! Bomb!";
        }
        else
        {
            _audioService.PlaySound("pop.mp3");
        }
        
        Score += item.Points;
        if (Score < 0) Score = 0;

        if (Score >= _targetScore)
        {
            EndGame(true);
        }
    }

    private void EndGame(bool win)
    {
        _isPlaying = false;
        _gameTimer.Stop();
        StatusText = win ? "You Win!" : "Try Again";
        if(win) _gameManager.EndGame(true, 3);
    }

    [RelayCommand]
    public void MoveBasket(double x)
    {
        BasketX = x;
        // Clamp
        if (BasketX < 0) BasketX = 0;
        if (BasketX > GameWidth - BasketWidth) BasketX = GameWidth - BasketWidth;
    }
    
    public void Stop() => _gameTimer?.Stop();
}

public partial class FallingItem : ObservableObject
{
    public string Image { get; set; }
    public int Points { get; set; }
    
    [ObservableProperty]
    private double _x;
    
    [ObservableProperty]
    private double _y;
}
