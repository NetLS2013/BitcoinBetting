using System.Collections.ObjectModel;
using BitcoinBetting.Core.Models;

namespace BitcoinBetting.Core.ViewModels
{
    public class BettingViewModel
    {
        public ObservableCollection<BettingItemModel> BettingItems { get; set; }
            
        public BettingViewModel()
        {
            BettingItems = new ObservableCollection<BettingItemModel>(new[]
            {
                new BettingItemModel { Title = "start up", Description = "desc 1"},
                new BettingItemModel { Title = "settings", Description = "desc 2"}
            });
        }
    }
}