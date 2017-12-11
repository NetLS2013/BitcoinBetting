using System;
using System.Globalization;
using BitcoinBetting.Core.Models.ListItems;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Helpers
{
    public class PaymentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(String))
            {
                throw new InvalidOperationException("The target must be a String");
            }
            
            switch ((PaymentStatus) value)
            {
                case PaymentStatus.Confirmed: 
                    return "Confirmed";
                    
                case PaymentStatus.Unconfirmed: 
                    return "Unconfirmed";
                    
                default: 
                    return "None";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}