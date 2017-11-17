﻿using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views
{
    public partial class BettingPage : ContentPage
    {
        public ListView ListView;
        
        public BettingPage()
        {
            InitializeComponent();
            
            BindingContext = new BettingViewModel();
            ListView = BettingItemsListView;
        }
    }
}