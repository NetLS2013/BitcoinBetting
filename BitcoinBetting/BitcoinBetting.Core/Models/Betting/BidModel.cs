namespace BitcoinBetting.Core.Models.Betting
{
    public class BidModel
    {
        public int BettingId { get; set; }
        public int WalletId { get; set; }
        public bool Side { get; set; }
        public string PaymentAddress { get; set; }
    }
}