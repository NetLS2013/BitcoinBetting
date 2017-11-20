using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Core.Models;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Models.Results;
using BitcoinBetting.Core.Models.Settings;
using BitcoinBetting.Core.Models.User;
using BitcoinBetting.Core.Services;
using BitcoinBetting.Core.Services.Validations;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views;
using BitcoinBetting.Core.Views.Account;
using BitcoinBetting.Core.Views.Modals;
using Xamarin.Forms;

namespace BitcoinBetting.Core.ViewModels
{
    public class AddressesViewModel : BaseViewModel
    {
        private WalletModel walletModel;
        
        public ObservableCollection<AddressItemModel> AddressesItems { get; set; }
        
        public ICommand ModalAddressesCommand => new Command(async () => await ModalAddressesPage());
        public ICommand AddressCreateCommand => new Command(async () => await AddressCreate());
        public ICommand AddressCreateDismissCommand => new Command(async () => await AddressCreateDismiss());
        
        private IRequestProvider requestProvider { get; set; }
        
        private bool IsValid { get; set; }
        private INavigation Navigation { get; set;}
        private Page CurrentPage { get; set;}
        private ListView ListView { get; set;}
        
        private ValidatableObject<string> address;
        
        public AddressesViewModel(INavigation navigation, Page context, ListView listView)
        {
            this.Navigation = navigation;
            this.CurrentPage = context;
            this.ListView = listView;
            
            requestProvider = new RequestProvider();
            walletModel = new WalletModel();
            
            Task.Run(async () => await LoadAddressItems());
            
            address = new ValidatableObject<string>();
            
            AddValidations();
        }
        
        private MenuItemModel selectedItem;
        
        public MenuItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                
                OnPropertyChanged(nameof(SelectedItem));

                ListViewItemSelected(selectedItem);
            }
        }
        
        public ValidatableObject<string> Address
        {
            get
            {
                walletModel.Address = address.Value;
                
                return address;
            }
            set
            {
                if (address.Value != value.Value)
                {
                    address = value; 
                    
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        private void ListViewItemSelected(MenuItemModel item)
        {
            if (item == null)
                return;

            ListView.SelectedItem = null;
        }

        private async Task ModalAddressesPage()
        {
            await Navigation.PushModalAsync (new AddressCreatePage(this));
        }
        
        private async Task LoadAddressItems()
        {
            IsBusy = true;
            
            var result = await requestProvider
                .GetAsync<WalletResultModel>(GlobalSetting.Instance.AddressGetWaletEndpoint);

            if (!result.result)
            {
                await Application.Current.MainPage.DisplayAlert("Error!",
                    Environment.NewLine + result.Message, "Ok");
            }
            else
            {
                AddressesItems = new ObservableCollection<AddressItemModel>(result.list);
            }

            OnPropertyChanged(nameof(AddressesItems));
            
            IsBusy = false;
        }
        
        private async Task AddressCreate()
        {
            IsValid = Validate();

            if (!IsValid)
            {
                string error = string.Empty;

                error += Address.Errors.Count > 0 ? Environment.NewLine + Address.Errors[0] : string.Empty;

                await Application.Current.MainPage.DisplayAlert("Error!", error, "Ok");
            }
            else
            {
                try
                {
                    var result = await requestProvider
                        .PostAsync<WalletModel, Result>(GlobalSetting.Instance.AddressCreateWaletEndpoint, walletModel);

                    if (!result.result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!",
                            Environment.NewLine + result.Message, "Ok");
                    }
                    else
                    {
                        AddressesItems.Add(new AddressItemModel { Address = Address.Value });
                        
                        OnPropertyChanged(nameof(AddressesItems));
                        
                        await AddressCreateDismiss();
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", Environment.NewLine + e.Message,
                        "Ok");
                }
            }
        }

        private async Task AddressCreateDismiss()
        {
            await Navigation.PopModalAsync();
        }

        private bool Validate()
        {
            return Address.Validate();
        }

        private void AddValidations()
        {
            Address.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A address is required" });
        }
    }
}