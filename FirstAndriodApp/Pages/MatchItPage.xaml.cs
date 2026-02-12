using FirstAndriodApp.Models;
using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class MatchItPage : ContentPage
{
    double startX, startY;

    Image? dragGhost;
    Image? draggingSource;
    Rect originalBounds;

    public MatchItPage(MatchItPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    MatchItPageModel Model => (MatchItPageModel)BindingContext;

    public void Reset_Clicked(object sender, EventArgs e)
    {
        if (draggingSource != null)
            EndDrag(draggingSource);
        else if (dragGhost != null)
            EndDrag(dragGhost);

        ClearHighlights();
        Model.Reset();
    }

    void Left_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (sender is not Image img || img.BindingContext is not MatchItem item)
            return;

        if (string.IsNullOrEmpty(item.ObjectImage))
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                BeginDrag(img);
                startX = 0;
                startY = 0;
                img.Scale = 1.15;
                img.Opacity = 0.0;
                break;

            case GestureStatus.Running:
                if (dragGhost == null)
                    return;

                dragGhost.TranslationX = startX + e.TotalX;
                dragGhost.TranslationY = startY + e.TotalY;
                HighlightNearbyTarget(dragGhost);
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                ClearHighlights();
                if (dragGhost != null)
                    _ = HandleDropAsync(dragGhost);
                else
                    EndDrag(img);
                break;
        }
    }

    void BeginDrag(Image img)
    {
        if (DragOverlay == null)
            return;

        draggingSource = img;

        // Capture current absolute bounds to place the overlay ghost in the same spot.
        var p = GetAbsoluteCenter(img);
        originalBounds = new Rect(p.X - img.Width / 2, p.Y - img.Height / 2, img.Width, img.Height);

        dragGhost = new Image
        {
            Source = img.Source,
            Aspect = img.Aspect,
            HeightRequest = img.Height,
            WidthRequest = img.Width,
            Opacity = 0.95,
            Scale = 1.15,
            InputTransparent = true,
            ZIndex = 9999,
        };

        DragOverlay.InputTransparent = true;
        DragOverlay.Children.Add(dragGhost);
        AbsoluteLayout.SetLayoutBounds(dragGhost, originalBounds);

        startX = 0;
        startY = 0;
    }

    void EndDrag(Image img)
    {
        if (DragOverlay == null)
            return;

        if (dragGhost != null)
        {
            DragOverlay.Children.Remove(dragGhost);
            dragGhost = null;
        }

        if (draggingSource != null)
        {
            draggingSource.Opacity = 1;
            draggingSource.Scale = 1;
            draggingSource.TranslationX = 0;
            draggingSource.TranslationY = 0;
            draggingSource = null;
        }

        originalBounds = Rect.Zero;

        img.ZIndex = 0;
        img.Opacity = 1;
        img.Scale = 1;
        img.TranslationX = 0;
        img.TranslationY = 0;
    }

    void HighlightNearbyTarget(Image dragged)
    {
        if (RightStack == null)
            return;

        var dragCenter = GetAbsoluteCenter(dragged);

        foreach (var child in RightStack.Children)
        {
            if (child is Border border && border.BindingContext is MatchItem item && !item.IsMatched)
            {
                var borderCenter = GetAbsoluteCenter(border);
                var distance = Math.Sqrt(
                    Math.Pow(dragCenter.X - borderCenter.X, 2) +
                    Math.Pow(dragCenter.Y - borderCenter.Y, 2));

                if (distance < 120)
                {
                    border.BackgroundColor = Color.FromArgb("#E8F5E9");
                    border.Scale = 1.05;
                }
                else
                {
                    border.BackgroundColor = Color.FromArgb("#FFEFEFEF");
                    border.Scale = 1.0;
                }
            }
        }
    }

    void ClearHighlights()
    {
        if (RightStack == null)
            return;

        foreach (var child in RightStack.Children)
        {
            if (child is Border border)
            {
                border.BackgroundColor = Color.FromArgb("#FFEFEFEF");
                border.Scale = 1.0;
            }
        }
    }

    async Task HandleDropAsync(Image dragged)
    {
        var source = draggingSource;

        if (source?.BindingContext is not MatchItem draggedItem)
        {
            EndDrag(source ?? dragged);
            return;
        }

        var target = FindNearestTarget(dragged);

        if (target == null)
        {
            Model.StatusText = "Drag to the right column to match!";
            EndDrag(source);
            return;
        }

        bool isMatch = await Model.CheckMatch(draggedItem.Id, target.Id);

        if (isMatch)
        {
            await AnimateSuccessAsync(target);
        }
        else
        {
            await ShakeTargetAsync(target);
        }
    }

    MatchItem? FindNearestTarget(Image dragged)
    {
        if (RightStack == null)
            return null;

        var dragCenter = GetAbsoluteCenter(dragged);
        MatchItem? nearest = null;
        double minDistance = 120;

        foreach (var child in RightStack.Children)
        {
            if (child is Border border && border.BindingContext is MatchItem item && !item.IsMatched)
            {
                var borderCenter = GetAbsoluteCenter(border);
                var distance = Math.Sqrt(
                    Math.Pow(dragCenter.X - borderCenter.X, 2) +
                    Math.Pow(dragCenter.Y - borderCenter.Y, 2));

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = item;
                }
            }
        }

        return nearest;
    }

    Border? FindBorderForItem(MatchItem item) =>
        RightStack?.Children.OfType<Border>().FirstOrDefault(b => ReferenceEquals(b.BindingContext, item));

    Point GetAbsoluteCenter(VisualElement element)
    {
        var x = element.X + element.Width / 2 + element.TranslationX;
        var y = element.Y + element.Height / 2 + element.TranslationY;

        var parent = element.Parent as VisualElement;
        while (parent != null && parent != this)
        {
            x += parent.X + parent.TranslationX;
            y += parent.Y + parent.TranslationY;
            parent = parent.Parent as VisualElement;
        }

        return new Point(x, y);
    }

    async Task AnimateSuccessAsync(MatchItem item)
    {
        var border = FindBorderForItem(item);
        if (border != null)
        {
            await FirstAndriodApp.Utilities.AnimationHelper.CelebrationBurst(border);
            border.BackgroundColor = Color.FromArgb("#C8E6C9");
        }
    }

    async Task ShakeTargetAsync(MatchItem item)
    {
        var border = FindBorderForItem(item);
        if (border != null)
        {
            await FirstAndriodApp.Utilities.AnimationHelper.ErrorFlash(border);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Content != null)
            await FirstAndriodApp.Utilities.AnimationHelper.PageEntrance(Content);
    }
}
