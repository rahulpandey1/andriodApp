using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class SpotDifferencePage : ContentPage
{
    public SpotDifferencePage(SpotDifferencePageModel model)
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
