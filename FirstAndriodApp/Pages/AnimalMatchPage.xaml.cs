using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class AnimalMatchPage : ContentPage
{
    public AnimalMatchPage(AnimalMatchPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
