using System;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using eBrokerDB;
using eBrokerDB.Models;
using eBrokerDBRepository.Operations;
using Codes;
using System.Collections.Generic;
using System.Linq;
using Traders.Operations;

namespace eBroker.Tests
{
    public class RepositoryUnitTest
    {
        DbContextOptions options;

        public RepositoryUnitTest()
        {
            options = new DbContextOptionsBuilder<EBrokerDBContext>().UseInMemoryDatabase("EbrokerDBTest").Options;
            using (var context = new EBrokerDBContext(options))
            {
                context.Equities.Add(new Equity() { Id = 1, Name = "Nagarro", Price = 3500 });
                context.Equities.Add(new Equity() { Id = 2, Name = "TCS", Price = 3000 });
                context.Equities.Add(new Equity() { Id = 3, Name = "TataSteel", Price = 1200 });

                context.Traders.Add(new Trader()
                {
                    Id = 1,
                    Name = "Champion",
                    Funds = 100000,
                    Holdings = "1,10;2,5"
                });
                context.Traders.Add(new Trader()
                {
                    Id = 2,
                    Name = "Hero",
                    Funds = 200000,
                    Holdings = "2,10;3,5"
                });
                context.Traders.Add(new Trader()
                {
                    Id = 3,
                    Name = "Chris",
                    Funds = 350000,
                    Holdings = "1,5;2,5"
                });

                context.SaveChanges();
            }
        }

        [Fact]
        public void GetsAllEquities()
        {
            using(var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                bool result = traderRepository.GetEquities().Count == 3;

                Assert.True(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetsSingleEquity()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                bool result = traderRepository.GetEquity(1).Name.Equals("Nagarro");

                Assert.True(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetsSingleEquity_Fail_EquityNotFound()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                bool result = traderRepository.GetEquity(4).Name.Equals("Error:" + ErrorCodes.EquityNotFound);

                Assert.True(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetsAllTraders()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                bool result = traderRepository.GetTraders().Count == 3;

                Assert.True(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetsSingleTrader()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                bool result = traderRepository.GetTrader(1).Name.Equals("Champion");

                Assert.True(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetsSingleTrader_Fail_TraderNotFound()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                bool result = traderRepository.GetTrader(4).Name.Equals("Error:" + ErrorCodes.TraderNotFound);

                Assert.True(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void AddsFundsSuccessfully()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                double old_amount = traderRepository.GetTrader(1).Funds;

                bool add_result = traderRepository.AddFunds(1, 1000);
                double new_amount = traderRepository.GetTrader(1).Funds;
                bool verify_add = new_amount - old_amount == 1000;

                Assert.True(add_result);
                Assert.True(verify_add);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void BuyEquity_New_Successfully()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);

                bool buy_result = traderRepository.BuyEquity(1, 3, 10);
                bool verify_new_equity = GetHoldings(traderRepository.GetTrader(1).Holdings).ContainsKey(3);
                bool verify_new_equity_quantity = GetHoldings(traderRepository.GetTrader(1).Holdings)[3] == 10;

                Assert.True(buy_result);
                Assert.True(verify_new_equity);
                Assert.True(verify_new_equity_quantity);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void BuyEquity_Existing_Successfully()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                KeyValuePair<int, int> old_equity = GetHoldings(traderRepository.GetTrader(1).Holdings).First(x => x.Key == 1);

                bool buy_result = traderRepository.BuyEquity(1, 1, 10);
                bool verify_updated_equity = GetHoldings(traderRepository.GetTrader(1).Holdings).First(x => x.Key == 1).Value == old_equity.Value + 10;

                Assert.True(buy_result);
                Assert.True(verify_updated_equity);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void SellEquity_Successfully()
        {
            using (var context = new EBrokerDBContext(options))
            {
                TraderRepository traderRepository = new TraderRepository(context);
                Equity e = traderRepository.GetEquity(1);
                Trader t = traderRepository.GetTrader(1);
                KeyValuePair<int, int> old_equity = GetHoldings(t.Holdings).First(x => x.Key == 1);
                double b = TraderHelper.ReduceBrokerage(e.Price * 10);
                double old_funds = t.Funds;

                bool buy_result = traderRepository.SellEquity(1, 1, 10, b);
                bool verify_updated_equity = GetHoldings(t.Holdings).First(x => x.Key == 1).Value == old_equity.Value - 10;
                bool verify_updated_funds = t.Funds == old_funds + b;

                Assert.True(buy_result);
                Assert.True(verify_updated_equity);
                Assert.True(verify_updated_funds);

                context.Database.EnsureDeleted();
            }
        }

        public Dictionary<int, int> GetHoldings(String holdings)
        {
            Dictionary<int, int> d_holdings = new Dictionary<int, int>();
            foreach(String s in holdings.Split(";"))
            {
                int[] h = s.Split(",").Select(x => Convert.ToInt32(x)).ToArray();
                d_holdings.Add(Convert.ToInt32(h[0]), Convert.ToInt32(h[1]));
            }
            return d_holdings;
        }
    }
}
