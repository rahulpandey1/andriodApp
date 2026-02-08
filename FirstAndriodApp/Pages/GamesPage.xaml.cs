using FirstAndriodApp.PageModels;
using FirstAndriodApp.Models;

namespace FirstAndriodApp.Pages;

public partial class GamesPage : ContentPage
{
    public GamesPage(GamesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is GameTile selectedGame)
        {
            if (BindingContext is GamesViewModel viewModel)
            {
               await viewModel.OpenGameCommand.ExecuteAsync(selectedGame);
            }
            
            // Deselect UI
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
