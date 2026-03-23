using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace DSAapp.Helpers;

/// <summary>
/// Convierte el valor de la cadena "Estatus" al glifo de ícono correspondiente.
/// </summary>
public class EstatusToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var estatus = value as string ?? string.Empty;

        return estatus switch
        {
            "Pendiente"  => "\uE823",  // Clock icon
            "En Proceso" => "\uE768",  // Play/Progress icon
            "Atendido"   => "\uE73E",  // Checkmark icon
            "Archivado"  => "\uE7B8",  // Archive icon
            _            => "\uE946",  // Info icon
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
