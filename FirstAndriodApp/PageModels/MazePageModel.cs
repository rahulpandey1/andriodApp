using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;
using FirstAndriodApp.Services;
using System.Collections.ObjectModel;

namespace FirstAndriodApp.PageModels;

public partial class MazePageModel : ObservableObject
{
    private readonly LevelService _levelService;
    private readonly IGameManager _gameManager;
    private readonly IAudioService _audioService;

    [ObservableProperty]
    private string _statusText = "Reach the Flag!";

    [ObservableProperty]
    private ObservableCollection<MazeCell> _cells = new();

    [ObservableProperty]
    private int _rows;
    
    [ObservableProperty]
    private int _cols;

    private char[,] _maze;
    private int _playerR, _playerC;
    private int _endR, _endC;
    private int _currentLevelId = 1;

    public MazePageModel(LevelService levelService, IGameManager gameManager, IAudioService audioService)
    {
        _levelService = levelService;
        _gameManager = gameManager;
        _audioService = audioService;
        LoadLevel(1);
    }

    private async void LoadLevel(int levelId)
    {
        _currentLevelId = levelId;
        var levels = await _levelService.LoadLevelsAsync("mazerunner");
        var level = levels.FirstOrDefault(l => l.LevelId == levelId);

        if (level == null)
        {
            StatusText = "Maze Master!";
            return;
        }

        string mapStr = level.Items.First().CorrectMatch;
        ParseMaze(mapStr);
        StatusText = level.Difficulty;
    }

    private void ParseMaze(string mapStr)
    {
        var lines = mapStr.Split('|');
        Rows = lines.Length;
        Cols = lines[0].Length;
        _maze = new char[Rows, Cols];
        Cells.Clear();

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                char ch = lines[r][c];
                _maze[r, c] = ch;
                
                var cell = new MazeCell { Row = r, Col = c };
                if (ch == '#') cell.Color = Colors.DarkGray; // Wall
                else if (ch == 'S') 
                {
                    _playerR = r; _playerC = c;
                    cell.IsPlayer = true;
                }
                else if (ch == 'E') 
                {
                    _endR = r; _endC = c;
                    cell.Color = Colors.Red; // Flag
                    cell.IsGoal = true;
                }
                else cell.Color = Colors.White; // Path
                
                Cells.Add(cell);
            }
        }
    }

    public void Move(int dr, int dc)
    {
        int nr = _playerR + dr;
        int nc = _playerC + dc;

        if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols && _maze[nr, nc] != '#')
        {
            // Valid Move
            UpdateCell(_playerR, _playerC, false);
            _playerR = nr;
            _playerC = nc;
            UpdateCell(_playerR, _playerC, true);

            if (_playerR == _endR && _playerC == _endC)
            {
                _audioService.PlaySound("win.mp3");
                StatusText = "Level Complete!";
                // Next level
                Task.Delay(1000).ContinueWith(_ => MainThread.BeginInvokeOnMainThread(() => LoadLevel(_currentLevelId + 1)));
            }
        }
        else
        {
            _audioService.PlaySound("bump.mp3");
        }
    }

    private void UpdateCell(int r, int c, bool isPlayer)
    {
        var cell = Cells.FirstOrDefault(x => x.Row == r && x.Col == c);
        if (cell != null) cell.IsPlayer = isPlayer;
    }

    [RelayCommand]
    public void Reset() => LoadLevel(1);
    
    [RelayCommand]
    public void MoveUp() => Move(-1, 0);
    [RelayCommand]
    public void MoveDown() => Move(1, 0);
    [RelayCommand]
    public void MoveLeft() => Move(0, -1);
    [RelayCommand]
    public void MoveRight() => Move(0, 1);
}

public partial class MazeCell : ObservableObject
{
    public int Row { get; set; }
    public int Col { get; set; }
    public Color Color { get; set; }
    public bool IsGoal { get; set; }

    [ObservableProperty]
    private bool _isPlayer;
}
