using System.Globalization;

namespace CalorieCalculator.Converters;

/// <summary>
/// Конвертира string към bool — true ако стрингът НЕ е празен.
/// Използва се за IsVisible на ErrorMessage лейбъли.
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value as string);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Обръща bool стойност — използва се за деактивиране на бутони при IsBusy.
/// </summary>
public class InvertBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b ? !b : true;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b ? !b : false;
    }
}

/// <summary>
/// Конвертира bool към иконка ✓ или ✗ за индикаторите за парола.
/// </summary>
public class BoolToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "✓" : "✗";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертира bool към иконка на око — отворено при true, затворено при false.
/// </summary>
public class BoolToEyeIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "👁" : "👁‍🗨";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертира bool към цвят — зелен при true, сиво-лилав при false.
/// Използва се за оцветяване на изискванията за паролата.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? Color.FromArgb("#6BCB77") : Color.FromArgb("#9E7BB5");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// За progress bar стъпка — бяло при активна, полу-прозрачно при неактивна.
/// </summary>
public class BoolToStepColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#FFFFFF30");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Обратен от StepColor — бяло когато стъпка 1 НЕ е видима (значи вече сме на 2 или 3).
/// </summary>
public class BoolToStepInactiveConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // value = IsStep1Visible; ако е false значи сме поне на стъпка 2
        return value is false ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#FFFFFF30");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Цвят на таймера — бял при нормално, червен при изтекъл.
/// </summary>
public class BoolToTimerColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? Color.FromArgb("#FCA5A5") : Color.FromArgb("#FFFFFF");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}