using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressesPage : ContentPage
    {
        public ListView ListView;
        
        public AddressesPage()
        {
            InitializeComponent();
            
            BindingContext = new AddressesViewModel(Navigation, this);
            ListView = AddressesItemsListView;
        }
    }
}