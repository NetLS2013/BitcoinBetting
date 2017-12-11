using System;
using System.Linq;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BitcoinBetting.Core.Views.Menu
{
    public partial class MasterPage : MasterDetailPage
    {
        public MasterPage()
        {
            InitializeComponent();
            
            Master = new MenuPage(this);
        }
    }
}