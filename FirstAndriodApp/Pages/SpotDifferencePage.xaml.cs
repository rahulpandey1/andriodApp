using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class SpotDifferencePage : ContentPage
{
    public SpotDifferencePage(SpotDifferencePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
