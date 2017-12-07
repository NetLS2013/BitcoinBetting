using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.Betting;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Models.Results;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views.Modals;
using BitcoinBetting.Core.Views.Settings;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class BettingViewModel : BaseViewModel
    {
        public ObservableCollection<BettingItemModel> BettingItems { get; set; }
        
        public ICommand BetNoCommnad => new Command(async e => await ModalBettingPage(e as BettingItemModel, side: false));
        public ICommand BetYesCommnad => new Command(async e => await ModalBettingPage(e as BettingItemModel, side: true));
        
        public ICommand BetHistoryCommnad => new Command(async e => await ModalHistoryPage(e as BettingItemModel));

        public ICommand DismissModalCommand => new Command(async () => await DismissModal());
        
        public ICommand CreateBettingCommand => new Command(async () => await CreateBetting());
        public ICommand ChooseBitcoinAddressCommand => new Command(async () => await ChooseBitcoinAddress());
        
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
        
        private async Task ModalBettingPage(BettingItemModel bettingItem, bool side)
        {
            var item = bettingItem;
            
            item.Side = side;
            item.Address = String.Empty;
            
            SelectedItem = item;
                
            await Navigation.PushModalAsync (new BettingCreatePage(this));
        }
        
        private async Task ModalHistoryPage(BettingItemModel bettingItem)
        {
            SelectedItem = bettingItem;
            
            await Navigation.PushModalAsync(new HistoryPage(viewModelContext: this));
        }
        
        private async Task LoadAddressItems()
        {
            IsBusy = true;
             
            try
            {
                var result = await requestProvider
                    .GetAsync<BettingResultModel>(GlobalSetting.Instance.BettingGetEndpoint);
    
                Device.BeginInvokeOnMainThread(async () => {
                    if (!result.Result)
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
        
        private async Task CreateBetting()
        {
            IsBusy = true;
            
            IsValid = Validate();

            if (IsValid)
            {
                try
                {
                    var createBid = new BidModel
                    {
                        BettingId = SelectedItem.BettingId,
                        WalletId = SelectedItem.WalletId,
                        Side = SelectedItem.Side
                    };
                    
                    var result = await requestProvider
                        .PostAsync<BidModel, BidResultModel>(GlobalSetting.Instance.BidCreateEndpoint, createBid);

                    if (!result.Result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!",
                            Environment.NewLine + result.Message, "Ok");
                    }
                    else
                    {
                        SelectedItem.Address = result.bid.PaymentAddress;
                        
                        OnPropertyChanged(nameof(SelectedItem));
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", Environment.NewLine + e.Message,
                        "Ok");
                }
            }
            
            IsBusy = false;
        }
        
        private async Task ChooseBitcoinAddress()
        {
            await Navigation.PushModalAsync(new AddressesPage(viewModelContext: this));
        }
        
        private async Task DismissModal()
        {
            await Navigation.PopModalAsync();
        }
        
        private bool Validate()
        {
            if (String.IsNullOrWhiteSpace(SelectedItem.Address))
            {
                Application.Current.MainPage.DisplayAlert("Error!", "Address is required", "Ok");
                
                return false;
            }

            return true;
        }
    }
}