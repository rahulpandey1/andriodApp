namespace FirstAndriodApp.Models;

public sealed class GameTile
{
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string Route { get; init; }
    public string Image { get; init; } = "icon_game_default.png";
    public bool IsLocked { get; set; }
    public int Stars { get; set; }
    public Color BackgroundColor { get; set; } = Colors.White;
}
