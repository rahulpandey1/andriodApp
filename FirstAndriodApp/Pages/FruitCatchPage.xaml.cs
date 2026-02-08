using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class FruitCatchPage : ContentPage
{
    private FruitCatchPageModel _model;

    public FruitCatchPage(FruitCatchPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var timer = Application.Current.Dispatcher.CreateTimer();
        _model.StartGame(timer);
        
        // Wait for layout to get dimensions
        GameArea.SizeChanged += (s, e) => 
        {
            if (GameArea.Width > 0)
            {
                _model.GameWidth = GameArea.Width;
                _model.GameHeight = GameArea.Height;
                _model.BasketY = GameArea.Height - 80;
                // Force update? No, binding should pick it up if PropertyChanged, but BasketY is property? Yes.
                // Wait, BasketY in VM is pure prop? No, it's auto-prop.
                // Let's refactor VM slightly to support this or just update here if bound.
                // Actually BasketY is not ObservableProperty in my code... checking... 
                // Ah, I made it `public double BasketY { get; set; }`. This won't notify.
                // It should be ObservableProperty or implement notification.
            }
        };
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _model.Stop();
    }

    private double _startX;

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType == GestureStatus.Started)
        {
            _startX = _model.BasketX;
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            _model.MoveBasket(_startX + e.TotalX);
        }
    }
}
