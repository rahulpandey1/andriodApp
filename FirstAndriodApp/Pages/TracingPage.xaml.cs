using CommunityToolkit.Mvvm.Messaging;
using FirstAndriodApp.PageModels;
using Microsoft.Maui.Graphics;

namespace FirstAndriodApp.Pages;

public partial class TracingPage : ContentPage
{
    private TracingDrawable _drawable;

    public TracingPage(TracingPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        
        _drawable = new TracingDrawable();
        DrawingCanvas.Drawable = _drawable;

        WeakReferenceMessenger.Default.Register<ClearCanvasMessage>(this, (r, m) =>
        {
            _drawable.Clear();
            DrawingCanvas.Invalidate();
        });
    }

    public void Reset_Clicked(object sender, EventArgs e) 
    {
        if (BindingContext is TracingPageModel model) model.Reset();
    }

    private void DrawingCanvas_StartInteraction(object sender, TouchEventArgs e)
    {
        _drawable.StartPath(e.Touches.FirstOrDefault());
        DrawingCanvas.Invalidate();
    }

    private void DrawingCanvas_DragInteraction(object sender, TouchEventArgs e)
    {
         _drawable.AddPoint(e.Touches.FirstOrDefault());
         DrawingCanvas.Invalidate();
    }

    private void DrawingCanvas_EndInteraction(object sender, TouchEventArgs e)
    {
        // Path finished
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Content != null)
            await FirstAndriodApp.Utilities.AnimationHelper.PageEntrance(Content);
    }
}

public class TracingDrawable : IDrawable
{
    private List<List<PointF>> _paths = new();
    private List<PointF> _currentPath = new();

    public void StartPath(PointF p)
    {
        _currentPath = new List<PointF> { p };
        _paths.Add(_currentPath);
    }

    public void AddPoint(PointF p)
    {
        if (_currentPath != null) _currentPath.Add(p);
    }

    public void Clear()
    {
        _paths.Clear();
        _currentPath.Clear();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Blue;
        canvas.StrokeSize = 20;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        foreach (var path in _paths)
        {
            if (path.Count < 2) continue;
            
            PathF p = new PathF();
            p.MoveTo(path[0]);
            foreach(var point in path.Skip(1)) p.LineTo(point);
            
            canvas.DrawPath(p);
        }
    }
}
