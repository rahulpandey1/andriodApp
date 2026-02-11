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
        // Play background music when games list is shown
        _audioService.PlayMusic("bgm_games.mp3");
    }

    private void LoadGames()
    {
        Games = new ObservableCollection<GameTile>
        {
            new() 
            { 
                Title = "Match It", 
                Subtitle = "🧩 Fit the shapes", 
                Route = "matchit",
                Image = "game1_icon.png",
                BackgroundColor = Color.FromArgb("#FF6B8A"),  // Candy Pink
                IsLocked = false 
            },
            new() 
            { 
                Title = "Memory Flip", 
                Subtitle = "🃏 Find Pairs", 
                Route = "memoryflip",
                Image = "game2_icon.png",
                BackgroundColor = Color.FromArgb("#45D9C8"),  // Mint Turquoise
                IsLocked = false 
            },
            new() 
            { 
                Title = "Shadow Match", 
                Subtitle = "🌓 Match Shadows", 
                Route = "shadowmatch",
                Image = "game3_icon.png",
                BackgroundColor = Color.FromArgb("#FFB347"),  // Warm Orange
                IsLocked = false 
            },
            new() 
            { 
                Title = "Color Match", 
                Subtitle = "🎨 Find Colors", 
                Route = "colormatch",
                Image = "game4_icon.png",
                BackgroundColor = Color.FromArgb("#A66CFF"),  // Soft Purple
                IsLocked = false 
            },
            new() 
            { 
                Title = "Shape Sorter", 
                Subtitle = "📦 Sort to Bins", 
                Route = "shapesorter",
                Image = "game5_icon.png",
                BackgroundColor = Color.FromArgb("#6BCB77"),  // Fresh Green
                IsLocked = false 
            },
            new() 
            { 
                Title = "Animal Sounds", 
                Subtitle = "🐾 Listen & Match", 
                Route = "animalmatch",
                Image = "game6_icon.png",
                BackgroundColor = Color.FromArgb("#74C0FC"),  // Sky Blue
                IsLocked = false 
            },
            new() 
            { 
                Title = "Puzzle Slider", 
                Subtitle = "🔀 Slide to Solve", 
                Route = "puzzleslider",
                Image = "game7_icon.png",
                BackgroundColor = Color.FromArgb("#FF8C42"),  // Warm Orange
                IsLocked = false 
            },
            new() 
            { 
                Title = "Jigsaw Puzzle", 
                Subtitle = "🧩 Sort Pieces", 
                Route = "jigsawpuzzle",
                Image = "game8_icon.png",
                BackgroundColor = Color.FromArgb("#FF6B6B"),  // Coral Red
                IsLocked = false 
            },
            new() 
            { 
                Title = "Alphabet Tracing", 
                Subtitle = "✏️ Write Letters", 
                Route = "alphabettracing",
                Image = "game9_icon.png",
                BackgroundColor = Color.FromArgb("#D4A5FF"),  // Lavender
                IsLocked = false 
            },
            new() 
            { 
                Title = "Number Connect", 
                Subtitle = "🔢 Connect Dots", 
                Route = "connectdots",
                Image = "game10_icon.png",
                BackgroundColor = Color.FromArgb("#A8E063"),  // Lime Green
                IsLocked = false 
            },
            new() 
            { 
                Title = "Spot Difference", 
                Subtitle = "🔍 Find Odd One", 
                Route = "spotdifference",
                Image = "game11_icon.png",
                BackgroundColor = Color.FromArgb("#45D9C8"),  // Mint
                IsLocked = false 
            },
            new() 
            { 
                Title = "Color Mixing", 
                Subtitle = "🌈 Create Colors", 
                Route = "colormixing",
                Image = "game12_icon.png",
                BackgroundColor = Color.FromArgb("#FFB3C6"),  // Rose Pink
                IsLocked = false 
            },
            new() 
            { 
                Title = "Fruit Catch", 
                Subtitle = "🍎 Catch Items", 
                Route = "fruitcatch",
                Image = "game13_icon.png",
                BackgroundColor = Color.FromArgb("#FFD93D"),  // Sunshine Yellow
                IsLocked = false 
            },
            new() 
            { 
                Title = "Maze Runner", 
                Subtitle = "🏃 Find the Way", 
                Route = "mazerunner",
                Image = "game14_icon.png",
                BackgroundColor = Color.FromArgb("#7DEDCC"),  // Mint Green
                IsLocked = false 
            },
            new() 
            { 
                Title = "Balloon Pop", 
                Subtitle = "🎈 Count & Pop", 
                Route = "balloonpop",
                Image = "game15_icon.png",
                BackgroundColor = Color.FromArgb("#A66CFF"),  // Purple
                IsLocked = false 
            }
        };
    }

    [RelayCommand]
    private async Task OpenGame(GameTile game)
    {
        if (game.IsLocked)
        {
            await Shell.Current.DisplayAlert("🔒 Locked", "Complete previous games to unlock!", "OK");
            return;
        }

        if (!string.IsNullOrEmpty(game.Route))
        {
            _audioService.PlaySound("click.mp3");
            _audioService.StopMusic(); // Stop BGM when entering a game
            await Shell.Current.GoToAsync(game.Route);
        }
    }
}
