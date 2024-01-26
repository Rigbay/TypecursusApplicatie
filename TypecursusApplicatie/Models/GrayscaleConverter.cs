using System;
using System.Globalization;
using System.IO;
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

        public static BitmapImage ConvertImageToGrayscale(BitmapImage colorImage)
        {
            var grayscaleEffect = new FormatConvertedBitmap();
            grayscaleEffect.BeginInit();
            grayscaleEffect.Source = colorImage;
            grayscaleEffect.DestinationFormat = PixelFormats.Gray8; // Gray8 for grayscale
            grayscaleEffect.EndInit();

            return ConvertFormatConvertedBitmapToBitmapImage(grayscaleEffect);
        }

        private static BitmapImage ConvertFormatConvertedBitmapToBitmapImage(FormatConvertedBitmap formatConvertedBitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();

            encoder.Frames.Add(BitmapFrame.Create(formatConvertedBitmap));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
