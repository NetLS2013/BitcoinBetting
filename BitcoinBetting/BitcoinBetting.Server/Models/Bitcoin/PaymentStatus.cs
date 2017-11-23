namespace BitcoinBetting.Server.Models.Bitcoin
{
    public enum PaymentStatus : byte
    {
        None,
        Unconfirmed, 
        Confirmed
    }
}