using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FirstAndriodApp.Models;

public sealed class MemoryCard : INotifyPropertyChanged
{
    public required string PairId { get; init; }
    public required string Image { get; init; }

    bool isFlipped;
    public bool IsFlipped
    {
        get => isFlipped;
        set
        {
            if (isFlipped == value) return;
            isFlipped = value;
            OnPropertyChanged();
        }
    }

    bool isMatched;
    public bool IsMatched
    {
        get => isMatched;
        set
        {
            if (isMatched == value) return;
            isMatched = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
