using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Views.Modals
{
    public partial class BidInfoPage : ContentPage
    {
        public BidInfoPage(HistoryViewModel viewModel)
        {
            InitializeComponent();
            
            BindingContext = viewModel;
        }
    }
}