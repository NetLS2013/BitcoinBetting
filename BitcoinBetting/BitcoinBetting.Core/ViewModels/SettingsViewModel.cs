using System;
using System.Collections.ObjectModel;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.Settings;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        public ObservableCollection<MenuItemModel> SettingsItems { get; set; }
        
        private INavigation Navigation { get; set;}
        private Page CurrentPage { get; set;}
        private ListView ListView { get; set;}
        
        public SettingsViewModel(INavigation navigation, Page currentPage, ListView listView)
        {
            this.Navigation = navigation;
            this.CurrentPage = currentPage;
            this.ListView = listView;
            
            SettingsItems = new ObservableCollection<MenuItemModel>(new[]
            {
                new MenuItemModel { Title = "Bitcoin addresses", TargetType = typeof(AddressesPage)},
                new MenuItemModel { Title = "History", TargetType = typeof(HistoryPage)},
                new MenuItemModel { Title = "Help", TargetType = typeof(HelpPage)}
            });
        }
        
        private MenuItemModel selectedItem;
        
        public MenuItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                
                OnPropertyChanged("SelectedItem");

                ListViewItemSelected(selectedItem);
            }
        }

        private void ListViewItemSelected(MenuItemModel item)
        {
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            
            Navigation.PushAsync(page);

            ListView.SelectedItem = null;
        }
    }
}