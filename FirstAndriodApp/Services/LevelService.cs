using System.Text.Json;
using FirstAndriodApp.Models;

namespace FirstAndriodApp.Services;

public class LevelService
{
    public async Task<List<GameLevel>> LoadLevelsAsync(string gameId)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("levels.json");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();
        
        var allLevels = JsonSerializer.Deserialize<Dictionary<string, List<GameLevel>>>(contents);
        
        if (allLevels != null && allLevels.ContainsKey(gameId))
        {
            return allLevels[gameId];
        }
        
        return new List<GameLevel>();
    }
}

public class GameLevel
{
    public int LevelId { get; set; }
    public string Difficulty { get; set; } = "Easy";
    public int TargetScore { get; set; }
    public int TimeLimitSeconds { get; set; }
    public List<LevelItem> Items { get; set; } = new();
}

public class LevelItem
{
    public string Id { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string CorrectMatch { get; set; } = string.Empty;
}
