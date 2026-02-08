namespace FirstAndriodApp.Services;

public class GameManager : IGameManager
{
    private const string KEY_TOTAL_STARS = "total_stars";
    private const string PREF_LEVEL_PREFIX = "level_unlocked_";

    public int TotalStars => Preferences.Get(KEY_TOTAL_STARS, 0);

    public void AddStars(int count)
    {
        int current = TotalStars;
        Preferences.Set(KEY_TOTAL_STARS, current + count);
    }

    public bool IsLevelUnlocked(int gameId, int levelId)
    {
        if (levelId == 1) return true; // Level 1 always unlocked
        return Preferences.Get($"{PREF_LEVEL_PREFIX}{gameId}_{levelId}", false);
    }

    public void UnlockLevel(int gameId, int levelId)
    {
        Preferences.Set($"{PREF_LEVEL_PREFIX}{gameId}_{levelId}", true);
    }

    public void StartGame(string gameName)
    {
        System.Diagnostics.Debug.WriteLine($"Started Game: {gameName}");
    }

    public void EndGame(bool won, int starsEarned)
    {
        if (won)
        {
            AddStars(starsEarned);
            // Logic to unlock next level could go here or be called explicitly
        }
    }
}
