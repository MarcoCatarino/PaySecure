// Helpers/Converters.cs
using System.Globalization;
using PaySecure.Models;

namespace PaySecure.Helpers;

// Convertidor para invertir valores booleanos
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }
}

// Convertidor para verificar si string no es null o vacío
public class StringIsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Convertidor para tipo de transacción a icono
public class TransactionTypeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TransactionType type)
        {
            return type switch
            {
                TransactionType.Payment => "💳",
                TransactionType.Refund => "↩️",
                TransactionType.Transfer => "🔄",
                _ => "📄"
            };
        }
        return "📄";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Convertidor para cantidad a color
public class AmountToColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is decimal amount && values[1] is TransactionType type)
        {
            return type switch
            {
                TransactionType.Payment when amount > 0 => Colors.Green,
                TransactionType.Refund => Colors.Orange,
                TransactionType.Transfer => Colors.Blue,
                _ => Colors.Red
            };
        }
        return Colors.Gray;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Convertidor para estado a color
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TransactionStatus status)
        {
            return status switch
            {
                TransactionStatus.Completed => Colors.Green,
                TransactionStatus.Pending => Colors.Orange,
                TransactionStatus.Failed => Colors.Red,
                TransactionStatus.Cancelled => Colors.Gray,
                _ => Colors.LightGray
            };
        }
        return Colors.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Convertidor para bool a Sí/No
public class BoolToYesNoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Sí" : "No";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString()?.ToLower() == "sí";
    }
}