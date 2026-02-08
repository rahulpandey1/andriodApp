using FirstAndriodApp.Models;
using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class MemoryFlipPage : ContentPage
{
    public MemoryFlipPage(MemoryFlipPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    MemoryFlipPageModel Model => (MemoryFlipPageModel)BindingContext;

    public void Reset_Clicked(object sender, EventArgs e) => Model.Reset();

    async void Card_Clicked(object sender, EventArgs e)
    {
        if (((Button)sender).CommandParameter is not MemoryCard card)
            return;

        await Model.FlipAsync(card);

        if (Model.IsCompleted)
            await DisplayAlert("Completed", $"You finished in {Model.Moves} moves!", "OK");
    }
}
