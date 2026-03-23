using Microsoft.UI.Xaml.Data;

namespace DSAapp.Helpers;

public class EstatusToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string estatus)
        {
            return estatus.ToLower() switch
            {
                "pendiente" => "\uE823",   // Ícono de reloj
                "en proceso" => "\uE822",  // Ícono de engranaje
                "finalizado" => "\uE930",  // Ícono de check circular
                "cancelado" => "\uE711",   // Ícono de X
                _ => "\uE946"              // Ícono de información genérica
            };
        }
        return "\uE946";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
