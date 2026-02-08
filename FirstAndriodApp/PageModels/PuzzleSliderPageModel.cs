using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class PuzzleSliderPageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private ObservableCollection<PuzzleTile> _tiles = new();

    [ObservableProperty]
    private string _statusText = "Tap tiles to slide them into order.";

    [ObservableProperty]
    private int _moves;

    private int _gridSize = 3;
    private int _currentLevelId = 1;

    public PuzzleSliderPageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("puzzleslider");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            StatusText = "Game Completed!";
            return;
        }

        var imageName = level.Items.FirstOrDefault()?.Image ?? "puzzle_puppy";
        InitializePuzzle(imageName);
    }

    private void InitializePuzzle(string baseImageName)
    {
        var tempTiles = new List<PuzzleTile>();
        int count = _gridSize * _gridSize;

        for (int i = 0; i < count; i++)
        {
            // Assuming image names like puzzle_puppy_0.png, puzzle_puppy_1.png ... 
            // The last tile (count-1) is the empty slot.
            bool isEmpty = i == count - 1;
            tempTiles.Add(new PuzzleTile
            {
                Id = i, // Correct Position
                CurrentPosition = i,
                Image = isEmpty ? "" : $"{baseImageName}_{i}.png",
                IsEmpty = isEmpty
            });
        }

        Shuffle(tempTiles);
        
        Tiles.Clear();
        foreach (var t in tempTiles) Tiles.Add(t);
        
        Moves = 0;
        StatusText = "Tap adjacent tiles to move.";
    }

    private void Shuffle(List<PuzzleTile> tiles)
    {
        // Simple Shuffle: Perform valid random moves
        var rnd = new Random();
        int shuffleMoves = 50;
        
        while (shuffleMoves > 0)
        {
            // Find empty tile
            var empty = tiles.First(t => t.IsEmpty);
            int emptyPos = tiles.IndexOf(empty);
            int row = emptyPos / _gridSize;
            int col = emptyPos % _gridSize;

            var validNeighbors = new List<int>();
            if (row > 0) validNeighbors.Add(emptyPos - _gridSize); // Up
            if (row < _gridSize - 1) validNeighbors.Add(emptyPos + _gridSize); // Down
            if (col > 0) validNeighbors.Add(emptyPos - 1); // Left
            if (col < _gridSize - 1) validNeighbors.Add(emptyPos + 1); // Right
            
            if (validNeighbors.Count > 0)
            {
                int swapPos = validNeighbors[rnd.Next(validNeighbors.Count)];
                // Swap
                (tiles[emptyPos], tiles[swapPos]) = (tiles[swapPos], tiles[emptyPos]);
                shuffleMoves--;
            }
        }
    }

    [RelayCommand]
    public async Task TileTap(PuzzleTile tile)
    {
        if (tile.IsEmpty) return;

        int index = Tiles.IndexOf(tile);
        int row = index / _gridSize;
        int col = index % _gridSize;

        // Check neighbors for empty
        int emptyIndex = -1;

        // Up
        if (row > 0 && Tiles[index - _gridSize].IsEmpty) emptyIndex = index - _gridSize;
        // Down
        else if (row < _gridSize - 1 && Tiles[index + _gridSize].IsEmpty) emptyIndex = index + _gridSize;
        // Left
        else if (col > 0 && Tiles[index - 1].IsEmpty) emptyIndex = index - 1;
        // Right
        else if (col < _gridSize - 1 && Tiles[index + 1].IsEmpty) emptyIndex = index + 1;

        if (emptyIndex != -1)
        {
            _audioService.PlaySound("slide.mp3");
            
            // Swap in ObservableCollection
            var emptyTile = Tiles[emptyIndex];
            
            Tiles[emptyIndex] = tile;
            Tiles[index] = emptyTile;
            
            Moves++;
            
            CheckWin();
        }
    }

    private void CheckWin()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i].Id != i) return;
        }

        StatusText = "Puzzle Solved!";
        _audioService.PlaySound("win.mp3");
        _gameManager.EndGame(true, 3);
    }
    
    [RelayCommand]
    public void Reset() => InitializePuzzle("puzzle_puppy");
}

public class PuzzleTile
{
    public int Id { get; set; }
    public int CurrentPosition { get; set; }
    public string Image { get; set; }
    public bool IsEmpty { get; set; }
}
