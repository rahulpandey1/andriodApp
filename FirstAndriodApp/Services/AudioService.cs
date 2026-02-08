using CommunityToolkit.Maui.Views;

namespace FirstAndriodApp.Services;

public class AudioService : IAudioService
{
    private bool _isMusicOn = true;
    private bool _isSfxOn = true;
    // In a real implementation, we would hold references to MediaPlayers here
    
    public bool IsMusicOn => _isMusicOn;
    public bool IsSfxOn => _isSfxOn;

    public AudioService()
    {
        // Load preferences
        _isMusicOn = Preferences.Get("isMusicOn", true);
        _isSfxOn = Preferences.Get("isSfxOn", true);
    }

    public void PlayMusic(string fileName)
    {
        if (!_isMusicOn) return;
        // logic to play loop
        System.Diagnostics.Debug.WriteLine($"Playing Music: {fileName}");
    }

    public void StopMusic()
    {
         System.Diagnostics.Debug.WriteLine($"Stopping Music");
    }

    public void PlaySound(string fileName)
    {
        if (!_isSfxOn) return;
        // logic to play one shot
        System.Diagnostics.Debug.WriteLine($"Playing SFX: {fileName}");
    }

    public void ToggleMusic()
    {
        _isMusicOn = !_isMusicOn;
        Preferences.Set("isMusicOn", _isMusicOn);
        if (!_isMusicOn) StopMusic();
        // else ResumeMusic();
    }

    public void ToggleSfx()
    {
        _isSfxOn = !_isSfxOn;
        Preferences.Set("isSfxOn", _isSfxOn);
    }
}
