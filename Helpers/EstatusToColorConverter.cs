using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DSAapp.Helpers;

/// <summary>
/// Convierte el valor de la cadena "Estatus" a un SolidColorBrush para el fondo del StateBadge.
/// Pendiente → ámbar, En Proceso → azul, Atendido → verde, Archivado → gris.
/// </summary>
public class EstatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var estatus = value as string ?? string.Empty;

        return estatus switch
        {
            "Pendiente"  => new SolidColorBrush(Color.FromArgb(255, 196, 130, 14)),  // Amber/Goldenrod
            "En Proceso" => new SolidColorBrush(Color.FromArgb(255, 0, 120, 212)),   // Blue accent
            "Atendido"   => new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)),   // Green success
            "Archivado"  => new SolidColorBrush(Color.FromArgb(255, 96, 96, 96)),    // Gray neutral
            _            => new SolidColorBrush(Color.FromArgb(255, 96, 96, 96)),    // Default gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) 
        => throw new NotImplementedException();
}
