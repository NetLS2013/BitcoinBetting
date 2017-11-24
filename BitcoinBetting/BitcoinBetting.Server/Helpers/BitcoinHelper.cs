namespace BitcoinBetting.Server.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BitcoinBetting.Server.Models.Bitcoin;

    using HBitcoin.KeyManagement;

    using NBitcoin;

    using QBitNinja.Client;
    using QBitNinja.Client.Models;

    public class BitcoinHelper
    {
        public static void GetBalances(IEnumerable<AddressHistoryRecord> addressHistoryRecords, out Money confirmedBalance, out Money unconfirmedBalance)
        {
            confirmedBalance = Money.Zero;
            unconfirmedBalance = Money.Zero;
            foreach (var record in addressHistoryRecords)
            {
                if (record.Confirmed)
                {
                    confirmedBalance += record.Amount;
                }
                else
                {
                    unconfirmedBalance += record.Amount;
                }
            }
        }

        public static bool SelectCoins(ref HashSet<Coin> coinsToSpend, Money totalOutAmount, List<Coin> unspentCoins)
        {
            var haveEnough = false;
            foreach (var coin in unspentCoins.OrderByDescending(x => x.Amount))
            {
                coinsToSpend.Add(coin);

                // if doesn't reach amount, continue adding next coin
                if (coinsToSpend.Sum(x => x.Amount) < totalOutAmount)
                {
                    continue;
                }
                else
                {
                    haveEnough = true;
                    break;
                }
            }

            return haveEnough;
        }

        public static Dictionary<Coin, bool> GetUnspentCoins(IEnumerable<ISecret> secrets, Network network)
        {
            var unspentCoins = new Dictionary<Coin, bool>();
            foreach (var secret in secrets)
            {
                var destination = secret.PrivateKey.ScriptPubKey.GetDestinationAddress(network);

                var client = new QBitNinjaClient(network);
                var balanceModel = client.GetBalance(destination, unspentOnly: true).Result;
                foreach (var operation in balanceModel.Operations)
                {
                    foreach (var elem in operation.ReceivedCoins.Select(coin => coin as Coin))
                    {
                        unspentCoins.Add(elem, operation.Confirmations > 0);
                    }
                }
            }

            return unspentCoins;
        }

        public static Dictionary<uint256, List<BalanceOperation>> GetOperationsPerTransactions(Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerAddresses)
        {
            // Get all the unique operations
            var opSet = new HashSet<BalanceOperation>();

            foreach (var elem in operationsPerAddresses)
            {
                foreach (var op in elem.Value)
                {
                    opSet.Add(op);
                }
                
            }

            // Get all operations, grouped by transactions
            var operationsPerTransactions = new Dictionary<uint256, List<BalanceOperation>>();
            foreach (var op in opSet)
            {
                var txId = op.TransactionId;

                List<BalanceOperation> ol;
                if (operationsPerTransactions.TryGetValue(txId, out ol))
                {
                    ol.Add(op);
                    operationsPerTransactions[txId] = ol;
                }
                else
                {
                    operationsPerTransactions.Add(txId, new List<BalanceOperation> { op });
                }
            }

            return operationsPerTransactions;
        }

        //public static Dictionary<BitcoinAddress, List<BalanceOperation>> QueryOperationsPerSafeAddresses(Safe safe, Network network, int minUnusedKeys = 7, HdPathType? hdPathType = null)
        //{
        //    if (hdPathType == null)
        //    {
        //        Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerReceiveAddresses = QueryOperationsPerSafeAddresses(safe, network, minUnusedKeys, HdPathType.Receive);
        //        Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerChangeAddresses = QueryOperationsPerSafeAddresses(safe, network, minUnusedKeys, HdPathType.Change);

        //        var operationsPerAllAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
        //        foreach (var elem in operationsPerReceiveAddresses)
        //        {
        //            operationsPerAllAddresses.Add(elem.Key, elem.Value);
        //        }

        //        foreach (var elem in operationsPerChangeAddresses)
        //        {
        //            operationsPerAllAddresses.Add(elem.Key, elem.Value);
        //        }

        //        return operationsPerAllAddresses;
        //    }

        //    var addresses = safe.GetFirstNAddresses(minUnusedKeys, hdPathType.GetValueOrDefault());

        //    var operationsPerAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
        //    var unusedKeyCount = 0;
        //    foreach (var elem in QueryOperationsPerAddresses(addresses, network))
        //    {
        //        operationsPerAddresses.Add(elem.Key, elem.Value);
        //        if (elem.Value.Count == 0)
        //        {
        //            unusedKeyCount++;
        //        }
        //    }

        //    var startIndex = minUnusedKeys;
        //    while (unusedKeyCount < minUnusedKeys)
        //    {
        //        addresses = new List<BitcoinAddress>();
        //        for (int i = startIndex; i < startIndex + minUnusedKeys; i++)
        //        {
        //            addresses.Add(safe.GetAddress(i, hdPathType.GetValueOrDefault()));
        //        }

        //        foreach (var elem in QueryOperationsPerAddresses(addresses, network))
        //        {
        //            operationsPerAddresses.Add(elem.Key, elem.Value);
        //            if (elem.Value.Count == 0)
        //            {
        //                unusedKeyCount++;
        //            }
        //        }

        //        startIndex += minUnusedKeys;
        //    }

        //    return operationsPerAddresses;
        //}

        public static Dictionary<BitcoinAddress, List<BalanceOperation>> QueryOperationsPerSafeAddresses1(Safe safe, Network network, IEnumerable<int> addressIds, HdPathType? hdPathType = null)
        {
            if (hdPathType == null)
            {
                Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerReceiveAddresses = QueryOperationsPerSafeAddresses1(safe, network, addressIds, HdPathType.Receive);
                Dictionary<BitcoinAddress, List<BalanceOperation>> operationsPerChangeAddresses = QueryOperationsPerSafeAddresses1(safe, network, addressIds, HdPathType.Change);

                var operationsPerAllAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
                foreach (var elem in operationsPerReceiveAddresses)
                {
                    operationsPerAllAddresses.Add(elem.Key, elem.Value);
                }

                foreach (var elem in operationsPerChangeAddresses)
                {
                    operationsPerAllAddresses.Add(elem.Key, elem.Value);
                }

                return operationsPerAllAddresses;
            }

            var addresses = new List<BitcoinAddress>();
            foreach (var id in addressIds)
            {
                addresses.Add(safe.GetAddress(id, hdPathType.GetValueOrDefault()));
            }

            var operationsPerAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
            foreach (var elem in QueryOperationsPerAddresses(addresses, network))
            {
                operationsPerAddresses.Add(elem.Key, elem.Value);
            }

            return operationsPerAddresses;
        }

        public static Dictionary<BitcoinAddress, List<BalanceOperation>> QueryOperationsPerAddresses(IEnumerable<BitcoinAddress> addresses, Network network)
        {
            var operationsPerAddresses = new Dictionary<BitcoinAddress, List<BalanceOperation>>();
            var client = new QBitNinjaClient(network);

            foreach (var address in addresses)
            {
                var operations = client.GetBalance(address, unspentOnly: false).Result.Operations;
                operationsPerAddresses.Add(address, operations);
            }

            return operationsPerAddresses;
        }

        public static Safe DecryptWalletByAskingForPassword(string walletFilePath, string password)
        {
            Safe safe = null;
            try
            {
                safe = Safe.Load(password, walletFilePath);
            }
            catch (System.Security.SecurityException)
            {

            }

            if (safe == null)
            {
                throw new Exception("Wallet could not be decrypted.");
            }

            return safe;
        }
    }
}