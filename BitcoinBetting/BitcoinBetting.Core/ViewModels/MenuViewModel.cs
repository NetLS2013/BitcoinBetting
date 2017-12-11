using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Models.Results;
using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.Account;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    class MenuViewModel : BaseViewModel
    {
        public ObservableCollection<MenuItemModel> MenuItems { get; set; }
        
        private IRequestProvider requestProvider { get; set; }
        
        private INavigation Navigation { get; set;}
        private MasterPage CurrentPage { get; set;}
        private ListView ListView { get; set;}
        
        public UserDataModel UserDataModel { get; set; }
        
        public MenuViewModel(INavigation navigation, MasterPage currentPage, ListView listView)
        {
            this.Navigation = navigation;
            this.CurrentPage = currentPage;
            this.ListView = listView;
            
            requestProvider = new RequestProvider();
            
            MenuItems = new ObservableCollection<MenuItemModel>(new[]
            {
                new MenuItemModel { Title = "Betting", TargetType = typeof(BettingPage)},
                new MenuItemModel { Title = "History", TargetType = typeof(HistoryPage)},
                new MenuItemModel { Title = "Settings", TargetType = typeof(SettingsPage)}
            });
            
            Task.Run(async () => await LoadUserData());
        }

        private async Task LoadUserData()
        {
            UserDataModel = await requestProvider.GetAsync<UserDataModel>(GlobalSetting.Instance.UserGetDataEndpoint);

            if (UserDataModel != null)
            {
                OnPropertyChanged("UserDataModel");
            }
        }

        private MenuItemModel selectedItem;
        
        public MenuItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;

                if (selectedItem != null)
                {
                    OnPropertyChanged("SelectedItem");

                    ListViewItemSelected(selectedItem);
                }
            }
        }

        private void ListViewItemSelected(MenuItemModel item)
        {
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            
            CurrentPage.Detail = new NavigationPage(page);
            CurrentPage.IsPresented = false;

            ListView.SelectedItem = null;
        }
    }
}