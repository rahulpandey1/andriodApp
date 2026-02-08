using FirstAndriodApp.Models;
using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class ShapeSorterPage : ContentPage
{
    public ShapeSorterPage(ShapeSorterPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    ShapeSorterPageModel Model => (ShapeSorterPageModel)BindingContext;

    public void Reset_Clicked(object sender, EventArgs e) => Model.Reset();

    // Drag Logic
    double startX, startY;
    Image? dragGhost;
    Image? draggingSource;
    Rect originalBounds;

    void Shape_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (sender is not Image img || img.BindingContext is not MatchItem item) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                BeginDrag(img);
                break;
            case GestureStatus.Running:
                if (dragGhost != null)
                {
                    dragGhost.TranslationX = startX + e.TotalX;
                    dragGhost.TranslationY = startY + e.TotalY;
                }
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (dragGhost != null) _ = HandleDropAsync(dragGhost);
                else EndDrag(img);
                break;
        }
    }

    void BeginDrag(Image img)
    {
        draggingSource = img;
        var p = GetAbsoluteCenter(img);
        originalBounds = new Rect(p.X - img.Width / 2, p.Y - img.Height / 2, img.Width, img.Height);

        dragGhost = new Image
        {
            Source = img.Source, Aspect = img.Aspect, HeightRequest = img.Height, WidthRequest = img.Width,
            Opacity = 0.9, Scale = 1.1, InputTransparent = true
        };
        DragOverlay.Children.Add(dragGhost);
        AbsoluteLayout.SetLayoutBounds(dragGhost, originalBounds);
        startX = 0; startY = 0;
        img.Opacity = 0;
    }

    void EndDrag(Image img)
    {
        if (dragGhost != null) { DragOverlay.Children.Remove(dragGhost); dragGhost = null; }
        if (draggingSource != null) { draggingSource.Opacity = 1; draggingSource = null; }
        img.Opacity = 1;
    }

    async Task HandleDropAsync(Image dragged)
    {
        var source = draggingSource;
        if (source?.BindingContext is not MatchItem item) { EndDrag(source ?? dragged); return; }

        string? binId = FindDroppedBin(dragged);
        
        if (binId != null)
        {
            bool isCorrect = await Model.CheckSort(item.Id, binId);
            if (isCorrect)
            {
                // Item removed by ViewModel, so we just clear ghost
            }
            else
            {
                // Shake or something
            }
        }

        EndDrag(source);
    }

    string? FindDroppedBin(Image dragged)
    {
        var dragCenter = GetAbsoluteCenter(dragged);
        // Bins are in Grid Row 3. Hardcoded checks for now based on screen thirds?
        // Or find constraints. Better: Check intersection with Grid Columns in Row 3.
        
        // Since Bins are hardcoded, I can check against a known valid Y range and X ranges.
        // But verifying visuals is safer.
        // Let's assume simpler hit testing:
        // Screen Width / 3.
        var width = this.Width;
        var third = width / 3;
        var yThreshold = this.Height - 150; // Bottom area

        if (dragCenter.Y > yThreshold)
        {
            if (dragCenter.X < third) return "Circle";
            if (dragCenter.X < third * 2) return "Square";
            return "Triangle";
        }
        return null;
    }

    Point GetAbsoluteCenter(VisualElement element)
    {
        // ... (Same implementation as before)
        var x = element.X + element.Width / 2 + element.TranslationX;
        var y = element.Y + element.Height / 2 + element.TranslationY;
        var parent = element.Parent as VisualElement;
        while (parent != null && parent != this) { x += parent.X + parent.TranslationX; y += parent.Y + parent.TranslationY; parent = parent.Parent as VisualElement; }
        return new Point(x, y);
    }
}
