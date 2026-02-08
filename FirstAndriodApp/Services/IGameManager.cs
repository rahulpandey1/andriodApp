namespace FirstAndriodApp.Services;

public interface IGameManager
{
    int TotalStars { get; }
    void AddStars(int count);
    bool IsLevelUnlocked(int gameId, int levelId);
    void UnlockLevel(int gameId, int levelId);
    
    // Helper for currently active game
    void StartGame(string gameName);
    void EndGame(bool won, int starsEarned);
}
