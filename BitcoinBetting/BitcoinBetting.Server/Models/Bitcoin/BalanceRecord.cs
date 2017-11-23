namespace BitcoinBetting.Server.Models.Bitcoin
{
    using NBitcoin;

    public class BalanceRecord
    {
        public BitcoinAddress BitcoinAddress { get; set; }

        public Money ConfirmMoney { get; set; }

        public Money UnconfirmMoney { get; set; }
    }
}