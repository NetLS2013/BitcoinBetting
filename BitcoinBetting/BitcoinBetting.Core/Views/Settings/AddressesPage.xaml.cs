using BitcoinBetting.Core.ViewModels;
using BitcoinBetting.Core.ViewModels.Base;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Views.Settings
{
    public partial class AddressesPage : ContentPage
    {
        public AddressesPage() : this(viewModelContext: null) { }
        
        public AddressesPage(BaseViewModel viewModelContext)
        {
            InitializeComponent();
            
            BindingContext = new AddressesViewModel(Navigation, this, AddressesItemsListView, viewModelContext);
        }
    }
}