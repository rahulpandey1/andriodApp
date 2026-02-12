using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class BalloonPopPage : ContentPage
{
    private BalloonPopPageModel _model;

    public BalloonPopPage(BalloonPopPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Content != null) 
            await FirstAndriodApp.Utilities.AnimationHelper.PageEntrance(Content);
        
        GameArea.SizeChanged += (s, e) => 
        {
            if (GameArea.Width > 0)
            {
                _model.GameWidth = GameArea.Width;
                _model.GameHeight = GameArea.Height;
            }
        };

        var timer = Application.Current.Dispatcher.CreateTimer();
        _model.StartGame(timer);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _model.Stop();
    }
}
