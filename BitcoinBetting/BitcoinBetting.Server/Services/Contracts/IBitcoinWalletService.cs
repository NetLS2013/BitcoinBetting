namespace BitcoinBetting.Server.Services.Contracts
{
    using System.Collections.Generic;

    using BitcoinBetting.Server.Models.Bitcoin;

    using NBitcoin;

    public interface IBitcoinWalletService
    {
        string[] GenerateWallet();

        List<TransactionHistoryRecord> GetHistory(IEnumerable<int> ids);

        IEnumerable<BitcoinAddress> GetAddresses(IEnumerable<int> ids);

        BitcoinAddress GetAddressById(int id);

        IEnumerable<BalanceRecord> GetBalances(
            IEnumerable<int> ids,
            out Money confirmedWalletBalance,
            out Money unconfirmedWalletBalance);

        void Send(string address, decimal amount, IEnumerable<int> ids, int idAddressSendRemainder);
    }
}