using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TypecursusApplicatie
{
    public class CompletionStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Black; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
