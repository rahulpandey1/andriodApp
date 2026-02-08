using FirstAndriodApp.Models;
using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class ShadowMatchPage : ContentPage
{
    public ShadowMatchPage(ShadowMatchPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    ShadowMatchPageModel Model => (ShadowMatchPageModel)BindingContext;

    // Drag Logic (Duplicate of MatchItPage, should be refactored into Behavior or Base Class ideally)
    
    double startX, startY;
    Image? dragGhost;
    Image? draggingSource;
    Rect originalBounds;

    public void Reset_Clicked(object sender, EventArgs e)
    {
        if (draggingSource != null) EndDrag(draggingSource);
        else if (dragGhost != null) EndDrag(dragGhost);
        ClearHighlights();
        Model.Reset();
    }

    void Left_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (sender is not Image img || img.BindingContext is not MatchItem item) return;
        if (string.IsNullOrEmpty(item.ObjectImage)) return;

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
                    HighlightNearbyTarget(dragGhost);
                }
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                ClearHighlights();
                if (dragGhost != null) _ = HandleDropAsync(dragGhost);
                else EndDrag(img);
                break;
        }
    }

    void BeginDrag(Image img)
    {
        if (DragOverlay == null) return;
        draggingSource = img;
        var p = GetAbsoluteCenter(img);
        originalBounds = new Rect(p.X - img.Width / 2, p.Y - img.Height / 2, img.Width, img.Height);

        dragGhost = new Image
        {
            Source = img.Source, Aspect = img.Aspect, HeightRequest = img.Height, WidthRequest = img.Width,
            Opacity = 0.95, Scale = 1.15, InputTransparent = true, ZIndex = 9999
        };
        DragOverlay.Children.Add(dragGhost);
        AbsoluteLayout.SetLayoutBounds(dragGhost, originalBounds);
        startX = 0; startY = 0;
        img.Opacity = 0;
    }

    void EndDrag(Image img)
    {
        if (DragOverlay == null) return;
        if (dragGhost != null) { DragOverlay.Children.Remove(dragGhost); dragGhost = null; }
        if (draggingSource != null) { draggingSource.Opacity = 1; draggingSource = null; }
        img.Opacity = 1; 
    }

    async Task HandleDropAsync(Image dragged)
    {
        var source = draggingSource;
        if (source?.BindingContext is not MatchItem draggedItem) { EndDrag(source ?? dragged); return; }

        var target = FindNearestTarget(dragged);
        if (target == null) { Model.StatusText = "Drag to the shadow!"; EndDrag(source); return; }

        bool isMatch = await Model.CheckMatch(draggedItem.Id, target.Id);
        if (isMatch) await AnimateSuccessAsync(target);
        else await ShakeTargetAsync(target);
        
        EndDrag(source);
    }

    // Helpers
    void HighlightNearbyTarget(Image dragged) { /* Implement highlight */ }
    void ClearHighlights() { /* Implement clear */ }
    
    MatchItem? FindNearestTarget(Image dragged)
    {
        if (RightStack == null) return null;
        var dragCenter = GetAbsoluteCenter(dragged);
        foreach (var child in RightStack.Children)
        {
            if (child is Border border && border.BindingContext is MatchItem item && !item.IsMatched)
            {
                var borderCenter = GetAbsoluteCenter(border);
                if (Math.Sqrt(Math.Pow(dragCenter.X - borderCenter.X, 2) + Math.Pow(dragCenter.Y - borderCenter.Y, 2)) < 120)
                    return item;
            }
        }
        return null;
    }

    Point GetAbsoluteCenter(VisualElement element)
    {
        var x = element.X + element.Width / 2 + element.TranslationX;
        var y = element.Y + element.Height / 2 + element.TranslationY;
        var parent = element.Parent as VisualElement;
        while (parent != null && parent != this) { x += parent.X + parent.TranslationX; y += parent.Y + parent.TranslationY; parent = parent.Parent as VisualElement; }
        return new Point(x, y);
    }

    async Task AnimateSuccessAsync(MatchItem item) { /* Animation */ }
    async Task ShakeTargetAsync(MatchItem item) { /* Animation */ }
}
