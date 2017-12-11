namespace BitcoinBetting.Server.Models
{
    using NBitcoin;

    public class BitcoinSettings
    {
        public Network Network => Network.GetNetwork(this.NetworkStr);

        public string NetworkStr { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }
    }
}