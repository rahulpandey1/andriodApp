using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class MazePage : ContentPage
{
    public MazePage(MazePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
