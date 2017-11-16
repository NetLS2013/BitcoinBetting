using System;

namespace BitcoinBetting.Core.Models.ListItems
{
    public class MenuItemModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Type TargetType { get; set; }
    }
}