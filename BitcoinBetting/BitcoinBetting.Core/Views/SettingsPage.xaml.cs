using System;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.ViewModels;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            
            BindingContext = new SettingsViewModel(Navigation, this, SettingsItemsListView);
        }
    }
}