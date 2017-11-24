using System;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.ViewModels;
using BitcoinBetting.Core.ViewModels.Base;
using BitcoinBetting.Core.Views.Menu;
using Xamarin.Forms;

namespace BitcoinBetting.Core
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage() : this(viewModelContext: null) { }
        
        public HistoryPage(BaseViewModel viewModelContext)
        {
            InitializeComponent();
            
            BindingContext = new HistoryViewModel(Navigation, this, BidItemsListView, viewModelContext);
        }
    }
}