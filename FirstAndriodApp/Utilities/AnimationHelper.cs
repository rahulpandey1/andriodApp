using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace FirstAndriodApp.Utilities;

public static class AnimationHelper
{
    /// <summary>
    /// Quick, snappy scale-in — starts at 80% and pops to 100%. 
    /// Much better than starting from 0 which feels broken.
    /// </summary>
    public static async Task QuickPopIn(VisualElement view, uint length = 250)
    {
        view.Scale = 0.8;
        view.Opacity = 1;
        view.IsVisible = true;
        await view.ScaleTo(1, length, Easing.SpringOut);
    }

    /// <summary>
    /// Fire-and-forget tap bounce — does NOT block calling code.
    /// Use: AnimationHelper.TapBounce(view);  (no await!)
    /// </summary>
    public static void TapBounce(VisualElement view)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await view.ScaleTo(0.92, 60, Easing.CubicOut);
                await view.ScaleTo(1.0, 60, Easing.CubicIn);
            }
            catch { /* ignore if view is disposed */ }
        });
    }

    /// <summary>
    /// Gentle page entrance — just a quick fade, no position change.
    /// Doesn't mess with TranslationY which can break touch targets.
    /// </summary>
    public static async Task PageEntrance(VisualElement view, uint length = 200)
    {
        view.Opacity = 0.3;
        await view.FadeTo(1, length, Easing.SinOut);
    }

    /// <summary>
    /// Shakes a view horizontally to indicate an error.
    /// </summary>
    public static async Task ShakeError(VisualElement view)
    {
        uint length = 40;
        await view.TranslateTo(-8, 0, length);
        await view.TranslateTo(8, 0, length);
        await view.TranslateTo(-4, 0, length);
        await view.TranslateTo(4, 0, length);
        await view.TranslateTo(0, 0, length);
    }

    /// <summary>
    /// Quick celebration — snappy scale pop, no rotation (rotation can mis-align layouts).
    /// </summary>
    public static async Task CelebrationBurst(VisualElement view)
    {
        await view.ScaleTo(1.15, 120, Easing.CubicOut);
        await view.ScaleTo(1.0, 120, Easing.BounceOut);
    }

    /// <summary>
    /// Creates a continuous gentle pulsing effect (breathing).
    /// </summary>
    public static async Task PulseLoop(VisualElement view, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await view.ScaleTo(1.03, 1200, Easing.SinInOut);
            if (token.IsCancellationRequested) break;
            await view.ScaleTo(0.97, 1200, Easing.SinInOut);
        }
        view.Scale = 1;
    }

    /// <summary>
    /// Red flash + shake for wrong answers.
    /// </summary>
    public static async Task ErrorFlash(VisualElement view)
    {
        var originalBg = view.BackgroundColor;
        view.BackgroundColor = Color.FromArgb("#FFCDD2");
        await ShakeError(view);
        await Task.Delay(150);
        view.BackgroundColor = originalBg;
    }

    /// <summary>
    /// Green flash for correct answers.
    /// </summary>
    public static async Task SuccessFlash(VisualElement view)
    {
        var originalBg = view.BackgroundColor;
        view.BackgroundColor = Color.FromArgb("#C8E6C9");
        await view.ScaleTo(1.08, 100, Easing.CubicOut);
        await view.ScaleTo(1.0, 100, Easing.CubicIn);
        view.BackgroundColor = originalBg;
    }

    /// <summary>
    /// Animated score increment effect — quick pop.
    /// </summary>
    public static async Task ScorePop(VisualElement scoreLabel)
    {
        await scoreLabel.ScaleTo(1.3, 80, Easing.CubicOut);
        await scoreLabel.ScaleTo(1.0, 120, Easing.BounceOut);
    }
}
