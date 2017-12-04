using System;
using System.Globalization;
using BitcoinBetting.Core.Models.ListItems;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Helpers
{
    public class PaymentAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");

            return !String.IsNullOrWhiteSpace((string) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}