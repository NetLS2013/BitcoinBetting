using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views
{
    public partial class BettingPage : ContentPage
    {
        public BettingPage()
        {
            InitializeComponent();
            
            BindingContext = new BettingViewModel(Navigation, this, BettingItemsListView);
        }
    }
}