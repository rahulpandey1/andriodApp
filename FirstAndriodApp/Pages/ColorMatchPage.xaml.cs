using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class ColorMatchPage : ContentPage
{
    public ColorMatchPage(ColorMatchPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    private void Reset_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is ColorMatchPageModel model)
            model.Reset();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Content != null)
            await FirstAndriodApp.Utilities.AnimationHelper.PageEntrance(Content);
    }
}
