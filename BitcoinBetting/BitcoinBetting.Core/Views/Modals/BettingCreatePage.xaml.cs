using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Views.Modals
{
    public partial class BettingCreatePage : ContentPage
    {
        public BettingCreatePage(BettingViewModel viewModel)
        {
            InitializeComponent();
            
            BindingContext = viewModel;
        }
    }
}