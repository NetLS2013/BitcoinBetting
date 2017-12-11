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
            return value != null && (value is string b ? !string.IsNullOrWhiteSpace(b) : throw new InvalidOperationException("The target must be a boolean"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}