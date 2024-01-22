using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TypecursusApplicatie.Converters
{
    public class GrayscaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imagePath = (string)value;
            var isUnlocked = (bool)value;
            if (!isUnlocked)
            {
                var grayscaleEffect = new FormatConvertedBitmap();
                grayscaleEffect.BeginInit();
                grayscaleEffect.Source = new BitmapImage(new Uri((string)parameter, UriKind.Relative));
                grayscaleEffect.DestinationFormat = PixelFormats.Gray32Float;
                grayscaleEffect.EndInit();
                return grayscaleEffect;
            }

            return new BitmapImage(new Uri((string)parameter, UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
