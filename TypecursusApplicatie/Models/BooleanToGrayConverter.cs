using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TypecursusApplicatie
{
    // BooleanToGrayConverter klasse, implementeert de IValueConverter interface
    // Deze converter wordt gebruikt om een boolean waarde om te zetten naar een kleur
    public class BooleanToGrayConverter : IValueConverter
    {
        // De Convert methode zet een boolean waarde om naar een SolidColorBrush (zwart of donkergrijs)
        // De methode krijgt een waarde, en retourneert zwart als de waarde true is, anders donkergrijs
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isUnlocked && isUnlocked) ? Brushes.Black : Brushes.DarkGray;
        }

        // ConvertBack methode is hier niet geïmplementeerd en zal een NotImplementedException gooien als het wordt aangeroepen
        // Dit is omdat de omgekeerde conversie (van kleur naar boolean) in deze context niet nodig is
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
