using System;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.ViewModels;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage()
        {
            InitializeComponent();
            
            //BindingContext = new HistoryViewModel(Navigation, this, SettingsItemsListView);
        }
    }
}