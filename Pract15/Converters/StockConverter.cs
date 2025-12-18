using System;
using System.Globalization;
using System.Windows.Data;

namespace Pract15.Converters
{
    public class StockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock == 0)
                    return "NoStock";
                else if (stock > 0 && stock < 10)
                    return "LowStock";
                else
                    return "Normal";
            }
            return "Normal";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}