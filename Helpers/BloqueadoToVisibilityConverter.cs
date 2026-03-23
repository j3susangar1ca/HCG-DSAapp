using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace DSAapp.Helpers;

/// <summary>
/// Convierte el valor de BloqueadoPor (string?) a Visibility.
/// Si no es null ni vacío, muestra el ícono de candado; de lo contrario, lo oculta.
/// </summary>
public class BloqueadoToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var bloqueadoPor = value as string;
        return string.IsNullOrEmpty(bloqueadoPor) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
