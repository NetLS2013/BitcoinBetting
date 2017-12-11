using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Views.Modals
{
    public partial class AddressCreatePage : ContentPage
    {
        public AddressCreatePage(AddressesViewModel viewModel)
        {
            InitializeComponent();
            
            BindingContext = viewModel;
        }
    }
}