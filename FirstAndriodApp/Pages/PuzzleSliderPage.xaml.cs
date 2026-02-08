using FirstAndriodApp.PageModels;
using System.Globalization;

namespace FirstAndriodApp.Pages;

public partial class PuzzleSliderPage : ContentPage
{
    public PuzzleSliderPage(PuzzleSliderPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value! ? Colors.Transparent : Colors.White;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !(bool)value!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
