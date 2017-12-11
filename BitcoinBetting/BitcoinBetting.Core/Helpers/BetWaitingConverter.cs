using System;
using System.Globalization;
using BitcoinBetting.Enum;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Helpers
{
    public class BetWaitingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(BettingStatus.Waiting);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
    
}