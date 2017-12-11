using System;
using System.Globalization;
using BitcoinBetting.Enum;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Helpers
{
    public class BetStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value != null && (value.Equals(BettingStatus.Waiting) || value.Equals(BettingStatus.Done)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}