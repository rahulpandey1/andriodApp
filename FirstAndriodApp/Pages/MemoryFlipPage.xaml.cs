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
        var button = (Button)sender;
        if (button.CommandParameter is not MemoryCard card)
            return;

        // Fire-and-forget tap feedback — does NOT delay the flip
        var cardContainer = (button.Parent as Grid)?.Parent as Border;
        if (cardContainer != null)
            FirstAndriodApp.Utilities.AnimationHelper.TapBounce(cardContainer);

        // Flip the card immediately
        await Model.FlipAsync(card);

        // Quick celebration if matched (after flip completes)
        if (card.IsMatched && cardContainer != null)
        {
            _ = FirstAndriodApp.Utilities.AnimationHelper.CelebrationBurst(cardContainer);
        }

        if (Model.IsCompleted)
        {
            await DisplayAlert("🎉 Amazing!", $"You finished in {Model.Moves} moves! ⭐", "OK");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Content != null)
            await FirstAndriodApp.Utilities.AnimationHelper.PageEntrance(Content);
    }
}
