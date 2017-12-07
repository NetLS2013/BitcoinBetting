using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models.Betting;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Models.Results;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views.Modals;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        public ObservableCollection<BidItemModel> BidItems { get; set; }
        
        public ICommand ModalBidInfoPageCommand => new Command(async e => await ModalBidInfoPage(e as BidItemModel));
        
        public ICommand DismissModalCommand => new Command(async () => await DismissModal());
        
        private IRequestProvider requestProvider { get; set; }
        
        private string urlLoadBidHistory;
        
        private bool IsValid { get; set; }
        
        public bool IsModalMode { get; set; }
        
        private BaseViewModel ViewModelContext { get; set; }
        
        private INavigation Navigation { get; set;}
        private Page CurrentPage { get; set;}
        private ListView ListView { get; set;}
        
        public HistoryViewModel(INavigation navigation, Page context, ListView listView, BaseViewModel viewModelContext = null)
        {
            this.Navigation = navigation;
            this.CurrentPage = context;
            this.ListView = listView;
            this.ViewModelContext = viewModelContext;
            
            requestProvider = new RequestProvider();
            
            if (ViewModelContext == null)
            {
                urlLoadBidHistory = GlobalSetting.Instance.BidGetEndpoint;
            }
            else
            {
                IsModalMode = true;
                    
                var bettingViewModel = ViewModelContext as BettingViewModel;

                if (bettingViewModel != null)
                {
                    urlLoadBidHistory = GlobalSetting.Instance.BidGetByIdEndpoint 
                                            + "?bettingId=" + bettingViewModel.SelectedItem.BettingId;
                }
            }
            
            Task.Run(async () => await LoadBidItems());
        }
        
        private BidItemModel selectedItem;
        
        public BidItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        
        private async Task LoadBidItems()
        {
            IsBusy = true;
             
            try
            {
                var result = await requestProvider.GetAsync<BidResultModel>(urlLoadBidHistory);
    
                Device.BeginInvokeOnMainThread(async () => {
                    if (!result.Result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!",
                            Environment.NewLine + result.Message, "Ok");
                    }
                    else
                    {
                        BidItems = new ObservableCollection<BidItemModel>(result.list);
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
            
            OnPropertyChanged(nameof(BidItems));
            
            IsBusy = false;
        }
        
        private async Task ModalBidInfoPage(BidItemModel bettingItem)
        {
            SelectedItem = bettingItem;
                
            await Navigation.PushModalAsync (new BidInfoPage(this));
        }
        
        private async Task DismissModal()
        {
            await Navigation.PopModalAsync();
        }
    }
}