using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DSAapp.Helpers;

public class EstatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string estatus)
        {
            return estatus.ToLower() switch
            {
                "pendiente" => new SolidColorBrush(Color.FromArgb(255, 255, 193, 7)),  // Ámbar
                "en proceso" => new SolidColorBrush(Color.FromArgb(255, 33, 150, 243)), // Azul
                "finalizado" => new SolidColorBrush(Color.FromArgb(255, 76, 175, 80)),  // Verde
                "cancelado" => new SolidColorBrush(Color.FromArgb(255, 244, 67, 54)),   // Rojo
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
