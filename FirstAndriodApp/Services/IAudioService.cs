namespace FirstAndriodApp.Services;

public interface IAudioService
{
    void PlayMusic(string fileName);
    void StopMusic();
    void PlaySound(string fileName);
    void ToggleMusic();
    void ToggleSfx();
    bool IsMusicOn { get; }
    bool IsSfxOn { get; }
}
