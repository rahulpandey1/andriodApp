using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class MazePage : ContentPage
{
    public MazePage(MazePageModel model)
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
