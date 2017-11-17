using System.Collections.ObjectModel;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Views;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    class AddressesViewModel
    {
        public ObservableCollection<AddressItemModel> AddressesItem { get; set; }
        
        private INavigation Navigation { get; set;}
        private Page CurrentPage { get; set;}
        
        public AddressesViewModel(INavigation navigation, Page currentPage)
        {
            this.Navigation = navigation;
            this.CurrentPage = currentPage;
            
            AddressesItem = new ObservableCollection<AddressItemModel>(new[]
            {
                new AddressItemModel { Address = "sadh123ahHSADasd21e312" },
                new AddressItemModel { Address = "t12adJ2saaAf545aASD12a" }
            });
        }
    }
}