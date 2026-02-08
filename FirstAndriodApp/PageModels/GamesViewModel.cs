using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class GamesViewModel : ObservableObject
{
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<GameTile> _games = new();

    public GamesViewModel(IGameManager gameManager, IAudioService audioService)
    {
        _gameManager = gameManager;
        _audioService = audioService;
        LoadGames();
    }

    private void LoadGames()
    {
        Games = new ObservableCollection<GameTile>
        {
            new() 
            { 
                Title = "Match It", 
                Subtitle = "Fit the shapes", 
                Route = "matchit",
                Image = "game1_icon.png",
                BackgroundColor = Color.FromArgb("#FF6B6B"), 
                IsLocked = false 
            },
            new() 
            { 
                Title = "Memory Flip", 
                Subtitle = "Find Pairs", 
                Route = "memoryflip",
                Image = "game2_icon.png",
                BackgroundColor = Color.FromArgb("#4ECDC4"), 
                IsLocked = false 
            },
            new() 
            { 
                Title = "Shadow Match", 
                Subtitle = "Match Shadows", 
                Route = "shadowmatch",
                Image = "game3_icon.png",
                BackgroundColor = Color.FromArgb("#FFE66D"), 
                IsLocked = false 
            },
            new() 
            { 
                Title = "Color Match", 
                Subtitle = "Find Colors", 
                Route = "colormatch",
                Image = "game4_icon.png",
                BackgroundColor = Color.FromArgb("#FF9F43"), 
                IsLocked = false 
            },
            new() 
            { 
                Title = "Shape Sorter", 
                Subtitle = "Sort to Bins", 
                Route = "shapesorter",
                Image = "game5_icon.png",
                BackgroundColor = Color.FromArgb("#A55EEA"), 
                IsLocked = false 
            },
            new() 
            { 
                Title = "Animal Sounds", 
                Subtitle = "Listen & Match", 
                Route = "animalmatch",
                Image = "game6_icon.png",
                BackgroundColor = Color.FromArgb("#26de81"),
                IsLocked = false 
            },
            new() 
            { 
                Title = "Puzzle Slider", 
                Subtitle = "Slide to Solve", 
                Route = "puzzleslider",
                Image = "game7_icon.png",
                BackgroundColor = Color.FromArgb("#45aaf2"), // Blue
                IsLocked = false 
            },
            new() 
            { 
                Title = "Jigsaw Puzzle", 
                Subtitle = "Sort Pieces", 
                Route = "jigsawpuzzle",
                Image = "game8_icon.png",
                BackgroundColor = Color.FromArgb("#fc5c65"), // Red
                IsLocked = false 
            },
            new() 
            { 
                Title = "Alphabet Tracing", 
                Subtitle = "Write Letters", 
                Route = "alphabettracing",
                Image = "game9_icon.png",
                BackgroundColor = Color.FromArgb("#fd9644"), // Orange
                IsLocked = false 
            },
            new() 
            { 
                Title = "Number Connect", 
                Subtitle = "Connect Dots", 
                Route = "connectdots",
                Image = "game10_icon.png",
                BackgroundColor = Color.FromArgb("#a55eea"), // Purple
                IsLocked = false 
            },
            new() 
            { 
                Title = "Spot Difference", 
                Subtitle = "Find Odd One", 
                Route = "spotdifference",
                Image = "game11_icon.png",
                BackgroundColor = Color.FromArgb("#3dc1d3"), // Light Blue
                IsLocked = false 
            },
            new() 
            { 
                Title = "Color Mixing", 
                Subtitle = "Create Colors", 
                Route = "colormixing",
                Image = "game12_icon.png",
                BackgroundColor = Color.FromArgb("#ff7979"), // Pinkish Red
                IsLocked = false 
            },
            new() 
            { 
                Title = "Fruit Catch", 
                Subtitle = "Catch Items", 
                Route = "fruitcatch",
                Image = "game13_icon.png",
                BackgroundColor = Color.FromArgb("#badc58"), // Lime Green
                IsLocked = false 
            },
            new() 
            { 
                Title = "Maze Runner", 
                Subtitle = "Find Logic", 
                Route = "mazerunner",
                Image = "game14_icon.png",
                BackgroundColor = Color.FromArgb("#f6e58d"), // Yellow
                IsLocked = false 
            },
            new() 
            { 
                Title = "Balloon Pop", 
                Subtitle = "Count & Pop", 
                Route = "balloonpop",
                Image = "game15_icon.png",
                BackgroundColor = Color.FromArgb("#686de0"), // Soft Purple
                IsLocked = false 
            }
        };
    }

    [RelayCommand]
    private async Task OpenGame(GameTile game)
    {
        if (game.IsLocked)
        {
            await Shell.Current.DisplayAlert("Locked", "Complete previous games to unlock!", "OK");
            return;
        }

        if (!string.IsNullOrEmpty(game.Route))
        {
            _audioService.PlaySound("click.mp3");
            await Shell.Current.GoToAsync(game.Route);
        }
    }
}
