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
            if (value == null)
                return false;


            if (value.Equals(BettingStatus.Waiting))
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}