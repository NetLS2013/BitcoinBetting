namespace BitcoinBetting.Server.Services.Bitcoin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;

    using BitcoinBetting.Server.Models;
    using BitcoinBetting.Server.Models.Bitcoin;

    using HBitcoin.KeyManagement;

    using NBitcoin;

    using Newtonsoft.Json.Linq;

    using QBitNinja.Client;
    using QBitNinja.Client.Models;

    public class BitcoinWalletService
    {
        private BitcoinSettings settings;

        public BitcoinWalletService(BitcoinSettings settings)
        {
            this.settings = settings;
        }

        public string[] GenerateWallet(string password, string path, Network network)
        {
            Safe.Create(out var mnemonic, password, path, network);

            return mnemonic.Words;
        }

        public List<TransactionHistoryRecord> GetHistory()
        {
            var safe = Helpers.BitcoinHelper.DecryptWalletByAskingForPassword(this.settings.Path, this.settings.Password);

            var operationsPerAddresses = Helpers.BitcoinHelper.QueryOperationsPerSafeAddresses(safe, this.settings.Network);
            var operationsPerTransactions = Helpers.BitcoinHelper.GetOperationsPerTransactions(operationsPerAddresses);

            var historyRecords = new List<TransactionHistoryRecord>();
            foreach (var elem in operationsPerTransactions)
            {
                var amount = Money.Zero;
                foreach (var op in elem.Value)
                {
                    amount += op.Amount;
                }

                var firstOp = elem.Value.First();

                historyRecords
                    .Add(new TransactionHistoryRecord() { Date = firstOp.FirstSeen.DateTime, Money = amount, IsConfirmed = firstOp.Confirmations > 0, TransactionId = elem.Key });
            }

            var orderedTxHistoryRecords = historyRecords
                .OrderByDescending(x => x.Date)
                .ThenBy(x => x.IsConfirmed);

            return orderedTxHistoryRecords.ToList();
        }

        public PaymentStatus IsPaymentDone(out uint256 transactionId, BitcoinAddress sender, BitcoinAddress receiver, decimal amount, DateTime? afterTime)
        {
            transactionId = null;

            var client = new QBitNinjaClient(Network.TestNet);

            var transactions = this.GetHistory().Where(record => record.Date >= (afterTime ?? DateTime.MinValue));

            foreach (var record in transactions)
            {
                if (record.Money.ToUnit(MoneyUnit.BTC) == amount && record.IsConfirmed)
                {
                    var transaction = client.GetTransaction(record.TransactionId).Result.Transaction;

                    var isCorrectSender = transaction.Inputs.AsIndexedInputs().FirstOrDefault(x => x.TxIn.ScriptSig.GetSignerAddress(this.settings.Network) == sender);
                    var isCorrectReceiver = transaction.Outputs.AsIndexedOutputs().FirstOrDefault(x => x.TxOut.ScriptPubKey.GetDestinationAddress(this.settings.Network) == receiver);

                    if (isCorrectSender != null && isCorrectReceiver != null)
                    {
                        transactionId = record.TransactionId;

                        return record.IsConfirmed ? PaymentStatus.Confirmed : PaymentStatus.Unconfirmed;
                    }
                }
            }

            return PaymentStatus.None;
        }

        //public PaymentStatus IsPaymentDone1(out uint256 transactionId, BitcoinAddress sender, BitcoinAddress receiver, decimal amount, DateTime? afterTime)
        //{
        //    transactionId = null;

        //    var client = new QBitNinjaClient(Network.TestNet);

        //    var transactions = this.GetHistory().Where(record => record.Date >= (afterTime ?? DateTime.MinValue));

        //    foreach (var record in transactions)
        //    {
        //        if (record.Money.ToUnit(MoneyUnit.BTC) == amount && record.IsConfirmed)
        //        {
        //            var transaction = client.GetTransaction(record.TransactionId).Result.Transaction;

        //            var isCorrectSender = transaction.Inputs.AsIndexedInputs().FirstOrDefault(x => x.TxIn.ScriptSig.GetSignerAddress(this.settings.Network) == sender);
        //            var isCorrectReceiver = transaction.Outputs.AsIndexedOutputs().FirstOrDefault(x => x.TxOut.ScriptPubKey.GetDestinationAddress(this.settings.Network) == receiver);

        //            if (isCorrectSender != null && isCorrectReceiver != null)
        //            {
        //                transactionId = record.TransactionId;

        //                return record.IsConfirmed ? PaymentStatus.Confirmed : PaymentStatus.Unconfirmed;
        //            }
        //        }
        //    }

        //    return PaymentStatus.None;
        //}

        public IEnumerable<BitcoinAddress> GetAddresses()
        {
            var safe = Helpers.BitcoinHelper.DecryptWalletByAskingForPassword(this.settings.Path, this.settings.Password);

            var operationsPerAddresses = Helpers.BitcoinHelper.QueryOperationsPerSafeAddresses(safe, this.settings.Network);

            foreach (var operationsPerAddress in operationsPerAddresses)
            {
                yield return operationsPerAddress.Key;
            }
        }

        public BitcoinAddress GetAddressById(int id)
        {
            var safe = Helpers.BitcoinHelper.DecryptWalletByAskingForPassword(this.settings.Path, this.settings.Password);

            var address = Helpers.BitcoinHelper.QueryOperationsPerSafeAddresses1(safe, this.settings.Network, new List<int>() { id }).FirstOrDefault();

            return address.Key;
        }

        public IEnumerable<BalanceRecord> GetBalances(out Money confirmedWalletBalance, out Money unconfirmedWalletBalance)
        {
            var safe = Helpers.BitcoinHelper.DecryptWalletByAskingForPassword(this.settings.Path, this.settings.Password);

            var operationsPerAddresses = Helpers.BitcoinHelper.QueryOperationsPerSafeAddresses(safe, this.settings.Network);

            var addressHistoryRecords = new List<AddressHistoryRecord>();
            foreach (var elem in operationsPerAddresses)
            {
                foreach (var op in elem.Value)
                {
                    addressHistoryRecords.Add(new AddressHistoryRecord(elem.Key, op));
                }
            }

            Helpers.BitcoinHelper.GetBalances(addressHistoryRecords, out confirmedWalletBalance, out unconfirmedWalletBalance);

            var addressHistoryRecordsPerAddresses = new Dictionary<BitcoinAddress, HashSet<AddressHistoryRecord>>();
            foreach (var address in operationsPerAddresses.Keys)
            {
                var recs = new HashSet<AddressHistoryRecord>();
                foreach (var record in addressHistoryRecords)
                {
                    if (record.Address == address)
                    {
                        recs.Add(record);
                    }
                }

                addressHistoryRecordsPerAddresses.Add(address, recs);
            }

            var listBalance = new List<BalanceRecord>();

            foreach (var elem in addressHistoryRecordsPerAddresses)
            {
                Money confirmedBalance;
                Money unconfirmedBalance;
                Helpers.BitcoinHelper.GetBalances(elem.Value, out confirmedBalance, out unconfirmedBalance);

                listBalance.Add(
                    new BalanceRecord()
                        {
                            BitcoinAddress = elem.Key,
                            ConfirmMoney = confirmedBalance,
                            UnconfirmMoney = unconfirmedBalance
                        });
            }

            return listBalance;
        }

        public IEnumerable<BalanceRecord> GetBalances1(IEnumerable<int> ids, out Money confirmedWalletBalance, out Money unconfirmedWalletBalance)
        {
            var safe = Helpers.BitcoinHelper.DecryptWalletByAskingForPassword(this.settings.Path, this.settings.Password);

            var operationsPerAddresses = Helpers.BitcoinHelper.QueryOperationsPerSafeAddresses1(safe, this.settings.Network, ids);

            var addressHistoryRecords = new List<AddressHistoryRecord>();
            foreach (var elem in operationsPerAddresses)
            {
                foreach (var op in elem.Value)
                {
                    addressHistoryRecords.Add(new AddressHistoryRecord(elem.Key, op));
                }
            }

            Helpers.BitcoinHelper.GetBalances(addressHistoryRecords, out confirmedWalletBalance, out unconfirmedWalletBalance);

            var addressHistoryRecordsPerAddresses = new Dictionary<BitcoinAddress, HashSet<AddressHistoryRecord>>();
            foreach (var address in operationsPerAddresses.Keys)
            {
                var recs = new HashSet<AddressHistoryRecord>();
                foreach (var record in addressHistoryRecords)
                {
                    if (record.Address == address)
                    {
                        recs.Add(record);
                    }
                }

                addressHistoryRecordsPerAddresses.Add(address, recs);
            }

            var listBalance = new List<BalanceRecord>();

            foreach (var elem in addressHistoryRecordsPerAddresses)
            {
                Money confirmedBalance;
                Money unconfirmedBalance;
                Helpers.BitcoinHelper.GetBalances(elem.Value, out confirmedBalance, out unconfirmedBalance);

                listBalance.Add(
                    new BalanceRecord()
                    {
                        BitcoinAddress = elem.Key,
                        ConfirmMoney = confirmedBalance,
                        UnconfirmMoney = unconfirmedBalance
                    });
            }

            return listBalance;
        }


        public void Send(string address, decimal amount)
        {
            var addressToSend = BitcoinAddress.Create(address, this.settings.Network);
            var safe = Helpers.BitcoinHelper.DecryptWalletByAskingForPassword(this.settings.Path, this.settings.Password);

            var operationsPerAddresses = Helpers.BitcoinHelper.QueryOperationsPerSafeAddresses(safe, this.settings.Network, 7);

            // Find all the not empty private keys
            var operationsPerNotEmptyPrivateKeys = new Dictionary<BitcoinExtKey, List<BalanceOperation>>();
            foreach (var elem in operationsPerAddresses)
            {
                var balance = Money.Zero;
                foreach (var op in elem.Value)
                {
                    balance += op.Amount;
                }

                if (balance > Money.Zero)
                {
                    var secret = safe.FindPrivateKey(elem.Key);
                    operationsPerNotEmptyPrivateKeys.Add(secret, elem.Value);
                }
            }

            // Get the script pubkey of the change.
            Script changeScriptPubKey = null;
            Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerChangeAddresses =
                Helpers.BitcoinHelper.QueryOperationsPerSafeAddresses(
                    safe,
                    this.settings.Network,
                    minUnusedKeys: 1,
                    hdPathType: HdPathType.Change);

            foreach (var elem in operationsPerChangeAddresses)
            {
                if (elem.Value.Count == 0)
                {
                    changeScriptPubKey = safe.FindPrivateKey(elem.Key).ScriptPubKey;
                }
            }

            if (changeScriptPubKey == null)
            {
                throw new ArgumentNullException();
            }

            var unspentCoins = Helpers.BitcoinHelper.GetUnspentCoins(
                operationsPerNotEmptyPrivateKeys.Keys,
                this.settings.Network);

            Money fee;
            try
            {
                var txSizeInBytes = 250;
                using (var client = new HttpClient())
                {
                    string request = @"https://bitcoinfees.21.co/api/v1/fees/recommended";
                    var result = client.GetAsync(request, HttpCompletionOption.ResponseContentRead).Result;
                    var json = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    var fastestSatoshiPerByteFee = json.Value<decimal>("fastestFee");
                    fee = new Money(fastestSatoshiPerByteFee * txSizeInBytes, MoneyUnit.Satoshi);
                }
            }
            catch
            {
                throw new Exception("Can't get tx fee");
            }

            Money availableAmount = Money.Zero;
            foreach (var elem in unspentCoins)
            {
                if (elem.Value)
                {
                    availableAmount += elem.Key.Amount;
                }
            }

            Money amountToSend = new Money(amount, MoneyUnit.BTC);

            if (amountToSend < Money.Zero || availableAmount < amountToSend + fee)
            {
                throw new Exception("Not enough coins.");
            }

            var totalOutAmount = amountToSend + fee;

            // Select coins
            var coinsToSpend = new HashSet<Coin>();
            var unspentConfirmedCoins = new List<Coin>();

            foreach (var elem in unspentCoins)
            {
                if (elem.Value)
                {
                    unspentConfirmedCoins.Add(elem.Key);
                }
            }
              

            bool haveEnough = Helpers.BitcoinHelper.SelectCoins(
                ref coinsToSpend,
                totalOutAmount,
                unspentConfirmedCoins);
            if (!haveEnough)
            {
                throw new Exception("Not enough funds.");
            }

            // Get signing keys
            var signingKeys = new HashSet<ISecret>();
            foreach (var coin in coinsToSpend)
            {
                foreach (var elem in operationsPerNotEmptyPrivateKeys)
                {
                    if (elem.Key.ScriptPubKey == coin.ScriptPubKey) signingKeys.Add(elem.Key);
                }
            }

            // Build the transaction
            var builder = new TransactionBuilder();
            var tx = builder.AddCoins(coinsToSpend).AddKeys(signingKeys.ToArray()).Send(addressToSend, amountToSend)
                .SetChange(changeScriptPubKey).SendFees(fee).BuildTransaction(true);

            if (!builder.Verify(tx))
            {
                throw new Exception("Couldn't build the transaction.");
            }

            var bitClient = new QBitNinjaClient(this.settings.Network);
            BroadcastResponse broadcastResponse;
            var success = false;
            var tried = 0;
            var maxTry = 7;
            do
            {
                tried++;

                broadcastResponse = bitClient.Broadcast(tx).Result;
                var getTxResp = bitClient.GetTransaction(tx.GetHash()).Result;
                if (getTxResp == null)
                {
                    Thread.Sleep(3000);
                    continue;
                }
                else
                {
                    success = true;
                    break;
                }
            }
            while (tried <= maxTry);

            if (!success)
            {
                if (broadcastResponse.Error != null)
                {
                    throw new Exception(
                        $"Error code: {broadcastResponse.Error.ErrorCode} Reason: {broadcastResponse.Error.Reason}");
                }
            }
        }
    }
}