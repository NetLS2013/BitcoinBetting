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
            if (value == null)
                return true;


            if (value.Equals(BettingStatus.Waiting) || value.Equals(BettingStatus.Done))
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}