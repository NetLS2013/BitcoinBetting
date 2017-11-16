using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.Account;

namespace BitcoinBetting.Core.ViewModels
{
    class MenuViewModel
    {
        public ObservableCollection<MenuItemModel> MenuItems { get; set; }
            
        public MenuViewModel()
        {
            MenuItems = new ObservableCollection<MenuItemModel>(new[]
            {
                new MenuItemModel { Id = 0, Title = "Betting", TargetType = typeof(BettingPage)},
                new MenuItemModel { Id = 1, Title = "Settings", TargetType = typeof(StartupPage)}
            });
        }
    }
}