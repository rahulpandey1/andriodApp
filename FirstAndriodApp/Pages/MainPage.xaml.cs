using FirstAndriodApp.Models;
using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}