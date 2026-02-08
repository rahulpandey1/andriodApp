using CommunityToolkit.Mvvm.Messaging;
using FirstAndriodApp.PageModels;
using Microsoft.Maui.Graphics;

namespace FirstAndriodApp.Pages;

public partial class ConnectDotsPage : ContentPage
{
    private ConnectDrawable _drawable;
    private DotItem? _startDot;

    public ConnectDotsPage(ConnectDotsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        
        _drawable = new ConnectDrawable();
        DrawingCanvas.Drawable = _drawable;
    }

    public void Reset_Clicked(object sender, EventArgs e) 
    {
        if (BindingContext is ConnectDotsPageModel model) 
        {
            model.Reset();
            _drawable.Clear();
            DrawingCanvas.Invalidate();
        }
    }

    private void DrawingCanvas_StartInteraction(object sender, TouchEventArgs e)
    {
        var touch = e.Touches.FirstOrDefault();
        _startDot = FindDotAt(touch);
        
        if (_startDot != null)
        {
            _drawable.CurrentLineStart = touch;
            _drawable.CurrentLineEnd = touch;
            DrawingCanvas.Invalidate();
        }
    }

    private void DrawingCanvas_DragInteraction(object sender, TouchEventArgs e)
    {
         if (_startDot == null) return;
         
         _drawable.CurrentLineEnd = e.Touches.FirstOrDefault();
         DrawingCanvas.Invalidate();
    }

    private void DrawingCanvas_EndInteraction(object sender, TouchEventArgs e)
    {
        if (_startDot == null) return;
        
        var endTouch = e.Touches.FirstOrDefault();
        var endDot = FindDotAt(endTouch);

        if (endDot != null && endDot != _startDot && BindingContext is ConnectDotsPageModel model)
        {
             if (model.TryConnect(_startDot.Id, endDot.Id))
             {
                 _drawable.AddLine(_startDot.Id, new PointF((float)_startDot.X+20, (float)_startDot.Y+20), new PointF((float)endDot.X+20, (float)endDot.Y+20));
                 _drawable.CurrentLineStart = PointF.Zero; // Hide current Drag
             }
        }
        
        _drawable.CurrentLineStart = PointF.Zero;
        DrawingCanvas.Invalidate();
        _startDot = null;
    }
    
    private DotItem? FindDotAt(PointF p)
    {
        // Simple hit test against DotItems in ViewModel
        if (BindingContext is ConnectDotsPageModel model)
        {
            foreach (var dot in model.Dots)
            {
                // Bounds check (Circle radius approx 20 at Translation X,Y)
                // Note: TranslationX/Y is top-left. Center is +20, +20.
                if (Math.Abs(p.X - (dot.X + 20)) < 30 && Math.Abs(p.Y - (dot.Y + 20)) < 30)
                {
                    return dot;
                }
            }
        }
        return null;
    }
}

public class ConnectDrawable : IDrawable
{
    private List<(PointF Start, PointF End)> _lines = new();
    public PointF CurrentLineStart { get; set; }
    public PointF CurrentLineEnd { get; set; }

    public void AddLine(int id, PointF start, PointF end)
    {
        _lines.Add((start, end));
    }

    public void Clear()
    {
        _lines.Clear();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 5;
        canvas.StrokeLineCap = LineCap.Round;

        foreach (var line in _lines)
        {
            canvas.DrawLine(line.Start, line.End);
        }

        if (CurrentLineStart != PointF.Zero)
        {
            canvas.StrokeColor = Colors.Blue;
            canvas.DrawLine(CurrentLineStart, CurrentLineEnd);
        }
    }
}
