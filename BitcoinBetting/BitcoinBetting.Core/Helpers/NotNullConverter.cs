using System;
using System.Globalization;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Helpers
{
    public class NotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}