using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class AnimalMatchPage : ContentPage
{
    public AnimalMatchPage(AnimalMatchPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Content != null)
            await FirstAndriodApp.Utilities.AnimationHelper.PageEntrance(Content);
    }
}
