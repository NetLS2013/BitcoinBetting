using System;

namespace BitcoinBetting.Core.Views.MasterDetail
{

    public class MenuItemPage
    {
        public MenuItemPage()
        {
            
        }
        
        public int Id { get; set; }
        public string Title { get; set; }
        public Type TargetType { get; set; }
    }
}