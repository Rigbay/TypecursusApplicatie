using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TypecursusApplicatie
{
    // BooleanToInverseVisibilityConverter klasse, implementeert IValueConverter
    // Deze converter wordt gebruikt om een boolean waarde om te zetten in een Visibility-waarde, maar dan omgekeerd
    public class BooleanToInverseVisibilityConverter : IValueConverter
    {
        // De Convert methode converteert een boolean naar een Visibility-waarde
        // Als de waarde true is, wordt Visibility.Collapsed geretourneerd, anders Visibility.Visible
        // Dit is het omgekeerde van wat je normaal zou verwachten
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        // ConvertBack methode is hier niet geïmplementeerd en zal een NotImplementedException gooien als het wordt aangeroepen
        // Dit is omdat de omgekeerde conversie (van Visibility naar boolean) in deze context niet nodig is
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
