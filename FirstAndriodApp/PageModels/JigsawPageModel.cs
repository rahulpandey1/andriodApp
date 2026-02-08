using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class JigsawPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<JigsawPiece> _pieces = new();
    
    [ObservableProperty]
    private int _boardSize = 300; // Fixed board size for simplicity

    [ObservableProperty]
    private string _statusText = "Drag pieces to the outline!";

    private int _gridSize = 2; // 2x2
    private int _placedCount;

    public JigsawPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        var levels = await _levelService.LoadLevelsAsync("jigsawpuzzle");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);
        
        if (level == null)
        {
            StatusText = "All Puzzles Solved!";
            return;
        }

        var baseName = level.Items.First().Image;
        InitializePuzzle(baseName);
    }

    private void InitializePuzzle(string baseName)
    {
        Pieces.Clear();
        int pieceSize = _boardSize / _gridSize;
        var rnd = new Random();

        for (int row = 0; row < _gridSize; row++)
        {
            for (int col = 0; col < _gridSize; col++)
            {
                // Target Positions (relative to Board TopLeft 0,0)
                double tX = col * pieceSize;
                double tY = row * pieceSize;

                // Random Scatter Positions (somewhere outside or around)
                // For simplicity, we'll put them in a "Tray" area below or scatter in view.
                // Let's randomize within a range 0-300 but ensure they aren't solved.
                double rX = rnd.Next(0, 50); 
                double rY = rnd.Next(320, 450); // Below board

                Pieces.Add(new JigsawPiece
                {
                    Id = $"{row}_{col}",
                    Image = $"{baseName}_{row}_{col}.png", // e.g. jigsaw_cat_0_0.png
                    TargetX = tX,
                    TargetY = tY,
                    CurrentX = rX + (col * 80), // Spread them out a bit
                    CurrentY = rY,
                    Width = pieceSize,
                    Height = pieceSize,
                    IsLocked = false
                });
            }
        }
        
        _placedCount = 0;
        StatusText = "Drag pieces to the board!";
    }

    public async Task<bool> TrySnap(JigsawPiece piece, double dropX, double dropY)
    {
        // Check distance to target
        // dropX/Y are relative to the Container (Board + Tray wrapper).
        // Board is at 0,0 of the interactive area? We need to align coord systems.
        // Assuming Page passes coordinates relative to the "Board" container.
        
        double dist = Math.Sqrt(Math.Pow(dropX - piece.TargetX, 2) + Math.Pow(dropY - piece.TargetY, 2));
        
        if (dist < 40) // Snap threshold
        {
            _audioService.PlaySound("snap.mp3");
            piece.CurrentX = piece.TargetX;
            piece.CurrentY = piece.TargetY;
            piece.IsLocked = true;
            piece.ZIndex = 0; // Send to back? Or lock in place.

            // Notify UI update manually or implement INPC in JigsawPiece
            // Re-assigning to trigger update if needed, but ObservableProperty on JigsawPiece is better.
            
            _placedCount++;
            if (_placedCount == _gridSize * _gridSize)
            {
                _audioService.PlaySound("win.mp3");
                _gameManager.EndGame(true, 3);
                StatusText = "Puzzle Completed!";
            }
            return true;
        }
        return false;
    }
    
    [RelayCommand]
    public void Reset() => InitializePuzzle("jigsaw_cat");
}

public partial class JigsawPiece : ObservableObject
{
    public string Id { get; set; }
    public string Image { get; set; }
    public double TargetX { get; set; }
    public double TargetY { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    
    [ObservableProperty]
    private double _currentX;
    
    [ObservableProperty]
    private double _currentY;
    
    [ObservableProperty]
    private bool _isLocked;
    
    [ObservableProperty]
    private int _zIndex = 10;
}
