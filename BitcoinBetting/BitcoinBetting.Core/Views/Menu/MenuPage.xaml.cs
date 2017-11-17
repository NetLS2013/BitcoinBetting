using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views.Menu
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage(MasterPage page)
        {
            InitializeComponent();

            BindingContext = new MenuViewModel(Navigation, page, MenuItemsListView);
        }
    }
}