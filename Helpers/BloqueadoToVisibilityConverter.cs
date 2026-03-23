using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace DSAapp.Helpers;

public class BloqueadoToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // Si hay un valor en BloqueadoPor (no es nulo ni vacío), mostramos el ícono de candado
        if (value is string bloqueadoPor && !string.IsNullOrWhiteSpace(bloqueadoPor))
        {
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
