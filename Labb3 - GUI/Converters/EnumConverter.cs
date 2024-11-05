using Labb3___GUI.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Labb3___GUI.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Difficulty enumValue)
            {
                return (int)enumValue;  // Convert enum to its corresponding index
            }
            return 0; // Default value if the enum is not valid
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index && Enum.IsDefined(typeof(Difficulty), index))
            {
                return (Difficulty)index; // Convert index back to enum
            }
            throw new InvalidOperationException("Invalid index value for Difficulty.");
        }
    }
}