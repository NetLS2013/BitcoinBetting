namespace BitcoinBetting.Server.Models
{
    using NBitcoin;

    public class BitcoinSettings
    {
        public Network Network { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }
    }
}