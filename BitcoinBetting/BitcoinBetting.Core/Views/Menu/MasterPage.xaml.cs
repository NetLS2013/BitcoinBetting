using System;
using BitcoinBetting.Core.Views.MasterDetail;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : MasterDetailPage
    {
        public MasterPage()
        {
            InitializeComponent();
            
            MenuPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuItemPage;
            
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MenuPage.ListView.SelectedItem = null;
        }
    }
}