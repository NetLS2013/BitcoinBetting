using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Models.Results;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views.Modals;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class BettingViewModel : BaseViewModel
    {
        public ObservableCollection<BettingItemModel> BettingItems { get; set; }
        
        public ICommand BetNoCommnad => new Command(async e => await ModalBettingPage(e as BettingItemModel, betType: false));
        public ICommand BetYesCommnad => new Command(async e => await ModalBettingPage(e as BettingItemModel, betType: true));

        public ICommand DismissModalCommand => new Command(async () => await DismissModal());
        
        private IRequestProvider requestProvider { get; set; }
        
        private bool IsValid { get; set; }
        private INavigation Navigation { get; set;}
        private Page CurrentPage { get; set;}
        private ListView ListView { get; set;}
        
        public BettingViewModel(INavigation navigation, Page context, ListView listView)
        {
            this.Navigation = navigation;
            this.CurrentPage = context;
            this.ListView = listView;
            
            requestProvider = new RequestProvider();
            
            Task.Run(async () => await LoadAddressItems());
        }
        
        private BettingItemModel selectedItem;
        
        public BettingItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        
        private async Task ModalBettingPage(BettingItemModel bettingItem, bool betType)
        {
            var item = bettingItem;
            item.BetType = betType;
            
            ListView.SelectedItem = item;
                
            await Navigation.PushModalAsync (new BettingCreatePage(this));
        }
        
        private async Task LoadAddressItems()
        {
            IsBusy = true;
             
            try
            {
                var result = await requestProvider
                    .GetAsync<BettingResultModel>(GlobalSetting.Instance.BettingGetEndpoint);
    
                Device.BeginInvokeOnMainThread(async () => {
                    if (!result.result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!",
                            Environment.NewLine + result.Message, "Ok");
                    }
                    else
                    {
                        BettingItems = new ObservableCollection<BettingItemModel>(result.list);
                    }
                });

            }
            catch (Exception e)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    await Application.Current.MainPage.DisplayAlert("Error!", Environment.NewLine + e.Message,
                        "Ok");
                });
            }
            
            OnPropertyChanged(nameof(BettingItems));
            
            IsBusy = false;
        }
        
        private async Task DismissModal()
        {
            await Navigation.PopModalAsync();
        }
    }
}