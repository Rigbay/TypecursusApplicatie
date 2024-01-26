using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TypecursusApplicatie
{
    // CompletionStatusToColorConverter klasse, implementeert IValueConverter
    // Deze converter wordt gebruikt om een boolean waarde (voltooiingsstatus) om te zetten naar een kleur
    public class CompletionStatusToColorConverter : IValueConverter
    {
        // De Convert methode converteert de boolean waarde naar een SolidColorBrush
        // Als de waarde true is (voltooid), wordt de kleur groen geretourneerd, anders rood
        // Als de waarde niet van het type bool is, wordt standaard zwart geretourneerd
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Black; // Standaard kleur
        }

        // ConvertBack methode is hier niet geïmplementeerd en zal een NotImplementedException gooien als het wordt aangeroepen
        // Dit is omdat de omgekeerde conversie (van kleur naar boolean) in deze context niet nodig is
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
