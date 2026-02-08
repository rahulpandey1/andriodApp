using CommunityToolkit.Mvvm.ComponentModel;

namespace FirstAndriodApp.Models;

public partial class MatchItem : ObservableObject
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    public string ObjectImage { get; set; } = string.Empty;
    public string ShadowImage { get; set; } = string.Empty; // Optional now

    [ObservableProperty]
    private bool _isMatched;
}
