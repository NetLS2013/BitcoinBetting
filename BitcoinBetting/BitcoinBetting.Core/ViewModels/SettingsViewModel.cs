using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.Account;
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
        
        private IRequestProvider requestProvider { get; set; }

        private string TitleSignout => "Sign out";
        
        public SettingsViewModel(INavigation navigation, Page currentPage, ListView listView)
        {
            this.Navigation = navigation;
            this.CurrentPage = currentPage;
            this.ListView = listView;
            
            requestProvider = new RequestProvider();
            
            SettingsItems = new ObservableCollection<MenuItemModel>(new[]
            {
                new MenuItemModel { Title = "Bitcoin addresses", TargetType = typeof(AddressesPage)},
                new MenuItemModel { Title = "Help", TargetType = typeof(HelpPage)},
                new MenuItemModel { Title = TitleSignout, TargetType = typeof(LoginPage)}
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

            var page = (Page) Activator.CreateInstance(item.TargetType);

            if (TitleSignout.Contains(item.Title))
            {
                SignOut();
                
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                Application.Current.Properties.Remove("token");
                Application.Current.Properties.Remove("refresh_token");
                
                return;
            }
            
            Navigation.PushAsync(page);

            ListView.SelectedItem = null;
        }
                
        private async Task SignOut()
        {
            try
            {
                await requestProvider.PostAsync(GlobalSetting.Instance.LogoutEndpoint);
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", Environment.NewLine + e.Message,
                    "Ok");
            }
        }
    }
}