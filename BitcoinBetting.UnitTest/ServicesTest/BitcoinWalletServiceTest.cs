namespace BitcoinBetting.UnitTest.ServicesTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using BitcoinBetting.Server.Helpers;
    using BitcoinBetting.Server.Models;
    using BitcoinBetting.Server.Services.Bitcoin;
    using BitcoinBetting.Server.Services.Contracts;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NBitcoin;

    [TestClass]
    public class BitcoinWalletServiceTest
    {
        private readonly IServiceProvider serviceProvider;

        private IBitcoinWalletService bitcoinWalletService;

        public BitcoinWalletServiceTest()
        {
            var services = new ServiceCollection();
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            services.AddTransient<IBitcoinAverageApi, BitcoinAverageApi>(
                provider => new BitcoinAverageApi(
                    new OptionsWrapper<BitcoinAvarageSettings>(
                        new BitcoinAvarageSettings() { SecretKey = "", PublicKey = "" })));

            this.bitcoinWalletService = new BitcoinWalletService(new BitcoinSettings() { NetworkStr = "TestNet", Password = "LgmAx4wStUxb7fU3", Path = "../../../wallet.dat"});
        }

        [TestMethod]
        public void GetAddresses_NoParams_ExpectEightAddresses()
        {
            var addressesAddresses = bitcoinWalletService.GetAddresses(new List<int>() { 0, 1, 2, 3 });

            Assert.AreEqual(addressesAddresses.Count(), 8);
            foreach (var addressesAddress in addressesAddresses)
            {
                //File.AppendAllText("D:/testcom.txt", addressesAddress.ToString() + Environment.NewLine);
                Assert.IsNotNull(addressesAddress.ToString());
            }
        }

        [TestMethod]
        public void GetHistory_NoParams_ExpectEightAddresses()
        {
            var addressesAddresses = bitcoinWalletService.GetHistory(new List<int>() { 0 });

            Assert.IsTrue(addressesAddresses.Any());
        }

        [TestMethod]
        public void Send_ValidParams_ExpectEightAddresses()
        {
            var address = this.bitcoinWalletService.GetAddressById(0);

            try
            {
                bitcoinWalletService.Send(address.ToString(), 0.0000001M, new List<int>(){ 0 }, 0);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void Send_InvalidParams_ExpectEightAddresses()
        {
            var address = this.bitcoinWalletService.GetAddressById(0);

            Assert.ThrowsException<Exception>(
                () => bitcoinWalletService.Send(address.ToString(), 1000M, new List<int>(0), 0));
        }

        [TestMethod]
        public void GetBalances_InvalidParams_ExpectEightAddresses()
        {
            var balances = bitcoinWalletService.GetBalances(new List<int>() { 0 }, out _, out _).FirstOrDefault();

            Assert.IsTrue(balances.ConfirmMoney > Money.Zero);
        }
    }
}