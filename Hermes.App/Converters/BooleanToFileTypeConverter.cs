using System.Globalization;
using System.Windows.Data;

namespace Hermes.App.Converters
{
    public class BooleanToFileTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isFolder)
                return isFolder ? "Folder" : "File";
            return "???";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string typeStr)
            {
                return typeStr.Equals("Folder", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}