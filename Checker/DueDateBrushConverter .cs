using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Checker
{
    public class DueDateBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime due)
            {
                var today = DateTime.Today;

                /**
                 * Farben werden zugewiesen. !WICHTIG Das hier ist nur teil der Logik! (Wenn jmd. die Farben aendern will musst du das im xaml machen)
                 * Colors are assigned. ! IMPORTANT: This is only part of the logic! (If sb. wants to change the colors you have to do it in the xaml)
                **/
                if (due < today) // Vergangen / Past
                    return "Red";
                if (due == today) // Heute / Today
                    return "White";
                if (due == today.AddDays(1)) // Morgen / Tomorrow
                    return "Violet";
                if (due < today.AddDays(7)) // Die woche / this week
                    return "Blue";
            }

            return "Gray"; // Standard
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
