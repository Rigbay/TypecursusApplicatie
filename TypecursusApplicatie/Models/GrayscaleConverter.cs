using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TypecursusApplicatie.Converters
{
    // GrayscaleConverter klasse, implementeert IValueConverter
    // Deze converter wordt gebruikt om een afbeeldingspad om te zetten naar een afbeelding, 
    // en indien aangegeven, de afbeelding in grijstinten weer te geven
    public class GrayscaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 'value' zou een bool moeten zijn die aangeeft of de afbeelding in kleur of grijstinten moet
            var isUnlocked = (bool)value;
            // 'parameter' zou het pad naar de afbeelding moeten zijn
            var imagePath = (string)parameter;

            if (!isUnlocked)
            {
                // Als 'isUnlocked' false is, converteer de afbeelding naar grijstinten
                var grayscaleEffect = new FormatConvertedBitmap();
                grayscaleEffect.BeginInit();
                grayscaleEffect.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                grayscaleEffect.DestinationFormat = PixelFormats.Gray32Float;
                grayscaleEffect.EndInit();
                return grayscaleEffect;
            }

            // Als 'isUnlocked' true is, retourneer de originele kleurenafbeelding
            return new BitmapImage(new Uri(imagePath, UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // Hulpmethode om een kleuren BitmapImage om te zetten naar een grijstinten BitmapImage
        public static BitmapImage ConvertImageToGrayscale(BitmapImage colorImage)
        {
            var grayscaleEffect = new FormatConvertedBitmap();
            grayscaleEffect.BeginInit();
            grayscaleEffect.Source = colorImage;
            grayscaleEffect.DestinationFormat = PixelFormats.Gray8; // Gray8 voor grijstinten
            grayscaleEffect.EndInit();

            return ConvertFormatConvertedBitmapToBitmapImage(grayscaleEffect);
        }

        // Hulpmethode om FormatConvertedBitmap naar BitmapImage om te zetten
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
