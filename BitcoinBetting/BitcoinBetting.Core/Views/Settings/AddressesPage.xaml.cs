using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Views.Settings
{
    public partial class AddressesPage : ContentPage
    {
        public AddressesPage()
        {
            InitializeComponent();
            
            BindingContext = new AddressesViewModel(Navigation, this, AddressesItemsListView);
        }
    }
}