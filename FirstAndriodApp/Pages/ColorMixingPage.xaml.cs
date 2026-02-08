using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class ColorMixingPage : ContentPage
{
    public ColorMixingPage(ColorMixingPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
