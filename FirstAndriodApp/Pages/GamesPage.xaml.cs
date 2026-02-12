using FirstAndriodApp.PageModels;
using FirstAndriodApp.Models;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAndriodApp.Pages;

public partial class GamesPage : ContentPage
{
    private bool _firstLoad = true;

    public GamesPage(GamesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_firstLoad)
        {
            _firstLoad = false;

            // Quick header pop-in (not from 0, just a subtle scale)
            if (HeaderLayout != null)
            {
                await FirstAndriodApp.Utilities.AnimationHelper.QuickPopIn(HeaderLayout);
            }

            // Quick fade for the collection
            if (GamesCollectionView != null)
            {
                await FirstAndriodApp.Utilities.AnimationHelper.PageEntrance(GamesCollectionView);
            }
        }
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is GameTile selectedGame)
        {
            var collectionView = (CollectionView)sender;
            
            // Fire-and-forget tap bounce — does NOT block navigation
            try 
            {
                var visualItem = collectionView.GetVisualTreeDescendants()
                                               .OfType<Border>()
                                               .FirstOrDefault(b => b.BindingContext == selectedGame);
                if (visualItem != null)
                    FirstAndriodApp.Utilities.AnimationHelper.TapBounce(visualItem);
            }
            catch { }

            // Navigate immediately — no waiting for animation
            if (BindingContext is GamesViewModel viewModel)
            {
                await viewModel.OpenGameCommand.ExecuteAsync(selectedGame);
            }
            
            collectionView.SelectedItem = null;
        }
    }
}
