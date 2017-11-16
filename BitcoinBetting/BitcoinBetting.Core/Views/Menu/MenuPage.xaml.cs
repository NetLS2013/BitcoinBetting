using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;

        public MenuPage()
        {
            InitializeComponent();

            BindingContext = new MenuViewModel();
            ListView = MenuItemsListView;
        }
    }
}