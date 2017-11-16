using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views.MasterDetail
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : ContentPage
    {
        public ListView ListView;

        public MasterPage()
        {
            InitializeComponent();

            BindingContext = new MasterViewModel();
            ListView = MenuItemsListView;
        }
    }
}