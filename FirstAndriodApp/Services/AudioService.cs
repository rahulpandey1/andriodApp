using Plugin.Maui.Audio;
using System.Diagnostics;

namespace FirstAndriodApp.Services;

public class AudioService : IAudioService
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _musicPlayer;
    private bool _isMusicOn = true;
    private bool _isSfxOn = true;
    private string? _currentMusicFile;

    public bool IsMusicOn => _isMusicOn;
    public bool IsSfxOn => _isSfxOn;

    public AudioService(IAudioManager audioManager)
    {
        _audioManager = audioManager;
        // Load preferences
        _isMusicOn = Preferences.Get("isMusicOn", true);
        _isSfxOn = Preferences.Get("isSfxOn", true);
    }

    public async void PlayMusic(string fileName)
    {
        if (!_isMusicOn) return;
        if (_currentMusicFile == fileName && _musicPlayer != null && _musicPlayer.IsPlaying)
            return; // Already playing this track

        try
        {
            StopMusic(); // Stop any current music first

            var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            _musicPlayer = _audioManager.CreatePlayer(stream);
            _musicPlayer.Loop = true;
            _musicPlayer.Volume = 0.3; // Keep BGM at lower volume
            _musicPlayer.Play();
            _currentMusicFile = fileName;
            Debug.WriteLine($"🎵 Playing Music: {fileName}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Could not play music '{fileName}': {ex.Message}");
        }
    }

    public void StopMusic()
    {
        try
        {
            if (_musicPlayer != null)
            {
                if (_musicPlayer.IsPlaying)
                    _musicPlayer.Stop();
                _musicPlayer.Dispose();
                _musicPlayer = null;
                _currentMusicFile = null;
                Debug.WriteLine("🔇 Music stopped");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Error stopping music: {ex.Message}");
        }
    }

    public async void PlaySound(string fileName)
    {
        if (!_isSfxOn) return;

        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            var player = _audioManager.CreatePlayer(stream);
            player.Volume = 0.7;
            player.Play();

            // Auto-dispose after the sound duration + buffer
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // Assume max 3s SFX
                try { player.Dispose(); } catch { }
            });

            Debug.WriteLine($"🔊 Playing SFX: {fileName}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Could not play sound '{fileName}': {ex.Message}");
        }
    }

    public void ToggleMusic()
    {
        _isMusicOn = !_isMusicOn;
        Preferences.Set("isMusicOn", _isMusicOn);
        if (!_isMusicOn)
            StopMusic();
        else if (_currentMusicFile != null)
            PlayMusic(_currentMusicFile);
        Debug.WriteLine($"🎶 Music: {(_isMusicOn ? "ON" : "OFF")}");
    }

    public void ToggleSfx()
    {
        _isSfxOn = !_isSfxOn;
        Preferences.Set("isSfxOn", _isSfxOn);
        Debug.WriteLine($"🔔 SFX: {(_isSfxOn ? "ON" : "OFF")}");
    }
}
