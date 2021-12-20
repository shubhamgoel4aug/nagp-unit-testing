using System;
using Xunit;
using Moq;
using eBrokerDBRepository.Interfaces;
using Traders.Operations;
using Traders.Mapper;
using AutoMapper;
using DB = eBrokerDB.Models;
using MD = Traders.Models;
using System.Collections.Generic;
using Codes;
using Traders.Interfaces;

namespace eBroker.Tests
{
    public class TraderUnitTest
    {
        Mock<ITraderRepository> traderRepository;
        Mock<IMapper> mapper;
        Mock<IWrapper> wrapper;
        TraderOperations traderOperations;

        List<MD.Equity> md_equities = new List<MD.Equity>()
        {
            new MD.Equity() { Id = 1, Name = "Equity1", Price = 100 },
            new MD.Equity() { Id = 2, Name = "Equity2", Price = 200 },
            new MD.Equity() { Id = 3, Name = "Equity3", Price = 300 }
        };

        List<DB.Equity> db_equities = new List<DB.Equity>()
        {
            new DB.Equity() { Id = 1, Name = "Equity1", Price = 100 },
            new DB.Equity() { Id = 2, Name = "Equity2", Price = 200 },
            new DB.Equity() { Id = 3, Name = "Equity3", Price = 300 }
        };

        List<MD.Trader> md_traders = new List<MD.Trader>()
        {
            new MD.Trader() { Id = 1, Name = "Trader1", Funds = 1000, Holdings = "1,5;2,10" },
            new MD.Trader() { Id = 2, Name = "Trader2", Funds = 2000, Holdings = "2,5;3,10" },
            new MD.Trader() { Id = 3, Name = "Trader3", Funds = 3000, Holdings = "1,5;3,10" }
        };

        List<DB.Trader> db_traders = new List<DB.Trader>()
        {
            new DB.Trader() { Id = 1, Name = "Trader1", Funds = 1000, Holdings = "1,5;2,10" },
            new DB.Trader() { Id = 2, Name = "Trader2", Funds = 2000, Holdings = "2,5;3,10" },
            new DB.Trader() { Id = 3, Name = "Trader3", Funds = 3000, Holdings = "1,5;3,10" }
        };

        public TraderUnitTest()
        {
            traderRepository = new Mock<ITraderRepository>();
            mapper = new Mock<IMapper>();
            wrapper = new Mock<IWrapper>();
            traderOperations = new TraderOperations(mapper.Object, traderRepository.Object, wrapper.Object);
        }

        [Fact]
        public void GetsAllEquities()
        {
            mapper.Setup(x => x.Map<List<MD.Equity>>(It.IsAny<DB.Equity>())).Returns(md_equities);
            bool result = traderOperations.GetEquities().Count > 0;
            Assert.True(result);
        }

        [Fact]
        public void GetsAllTraders()
        {
            mapper.Setup(x => x.Map<List<MD.Trader>>(It.IsAny<DB.Trader>())).Returns(md_traders);
            bool result = traderOperations.GetTraders().Count > 0;
            Assert.True(result);
        }

        [Fact]
        public void GetsSingleEquity()
        {
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            bool result = traderOperations.GetEquity(1).Id == 1;
            Assert.True(result);
        }

        [Fact]
        public void GetsSingleTrader()
        {
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            bool result = traderOperations.GetTrader(1).Id == 1;
            Assert.True(result);
        }

        [Fact]
        public void ShouldAddFunds()
        {
            traderRepository.Setup(x => x.AddFunds(It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            bool result = traderOperations.AddFunds(1, 1000).Equals("Success:" + SuccessCodes.FundsAdditionSuccess);
            Assert.True(result);
        }

        [Fact]
        public void AddFunds_ShouldReturn_TraderNotFound()
        {
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(new MD.Trader() { Id = 0, Name = "Error:" + ErrorCodes.TraderNotFound , Funds = 0, Holdings = ""});
            bool result = traderOperations.AddFunds(0, 10000).Equals("Error:" + ErrorCodes.TraderNotFound);
            Assert.True(result);
        }

        [Fact]
        public void AddFunds_ShouldReturn_FundsAdditionNegativeOr0()
        {
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            bool result = traderOperations.AddFunds(1, -10000).Equals("Error:" + ErrorCodes.FundsAdditionNegativeOr0);
            Assert.True(result);
        }

        [Fact]
        public void AddFunds_ShouldReturn_FundsAdditionFailure()
        {
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            traderRepository.Setup(x => x.AddFunds(It.IsAny<int>(), It.IsAny<double>())).Returns(false);
            bool result = traderOperations.AddFunds(1, 1000).Equals("Error:" + ErrorCodes.FundsAdditionFailure);
            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_ShouldReturn_TradeOutsideTimings()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(false);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(new MD.Trader() { Id = 0, Name = "Error:" + ErrorCodes.TraderNotFound, Funds = 0, Holdings = "" });
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            bool result = traderOperations.BuyEquity(0, 1, 5).Equals("Error:" + ErrorCodes.TradeOutsideTimings);
            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_Success()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            bool result = traderOperations.BuyEquity(1, 1, 5).Equals("Success:" + SuccessCodes.EquityBuySuccess);
            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_ShouldReturn_TraderNotFound()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(new MD.Trader() { Id = 0, Name = "Error:" + ErrorCodes.TraderNotFound, Funds = 0, Holdings = "" });
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            bool result = traderOperations.BuyEquity(0, 1, 5).Equals("Error:" + ErrorCodes.TraderNotFound);
            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_ShouldReturn_EquityNotFound()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(new MD.Equity() { Id = 0, Name = "Error:" + ErrorCodes.EquityNotFound, Price = 100 });
            traderRepository.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            bool result = traderOperations.BuyEquity(1, 0, 5).Equals("Error:" + ErrorCodes.EquityNotFound);
            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_ShouldReturn_EquityUnitsNegativeOr0()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            bool result = traderOperations.BuyEquity(1, 1, -5).Equals("Error:" + ErrorCodes.EquityUnitsNegativeOr0);
            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_ShouldReturn_EquityNotEnoughFunds()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            bool result = traderOperations.BuyEquity(1, 1, 100).Equals("Error:" + ErrorCodes.EquityNotEnoughFunds);
            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_ShouldReturn_EquityBuyFailed()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            bool result = traderOperations.BuyEquity(1, 1, 10).Equals("Error:" + ErrorCodes.EquityBuyFailed);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_ShouldReturn_TradeOutsideTimings()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(false);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            bool result = traderOperations.SellEquity(1, 1, 5).Equals("Error:" + ErrorCodes.TradeOutsideTimings);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_Successfully()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            bool result = traderOperations.SellEquity(1, 1, 5).Equals("Success:" + SuccessCodes.EquitySellSuccess);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_ShouldReturn_TraderNotFound()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(new MD.Trader() { Id = 0, Name = "Error:" + ErrorCodes.TraderNotFound, Funds = 0, Holdings = "" });
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            bool result = traderOperations.SellEquity(0, 1, 5).Equals("Error:" + ErrorCodes.TraderNotFound);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_ShouldReturn_EquityNotFound()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(new MD.Equity() { Id = 0, Name = "Error:" + ErrorCodes.EquityNotFound, Price = 100 });
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            bool result = traderOperations.SellEquity(1, 0, 5).Equals("Error:" + ErrorCodes.EquityNotFound);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_ShouldReturn_EquityUnitsNegativeOr0()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            bool result = traderOperations.SellEquity(1, 1, -5).Equals("Error:" + ErrorCodes.EquityUnitsNegativeOr0);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_ShouldReturn_EquityNotHeldByTrader()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            bool result = traderOperations.SellEquity(1, 3, 10).Equals("Error:" + ErrorCodes.EquityNotHeldByTrader);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_ShouldReturn_EquityQuantityNotHeldByTrader()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(true);
            bool result = traderOperations.SellEquity(1, 1, 100).Equals("Error:" + ErrorCodes.EquityQuantityNotHeldByTrader);
            Assert.True(result);
        }

        [Fact]
        public void SellEquity_ShouldReturn_EquitySellFailed()
        {
            wrapper.Setup(x => x.CheckBuySellTime()).Returns(true);
            mapper.Setup(x => x.Map<MD.Trader>(It.IsAny<DB.Trader>())).Returns(md_traders[0]);
            mapper.Setup(x => x.Map<MD.Equity>(It.IsAny<DB.Equity>())).Returns(md_equities[0]);
            traderRepository.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).Returns(false);
            bool result = traderOperations.SellEquity(1, 1, 5).Equals("Error:" + ErrorCodes.EquitySellFailed);
            Assert.True(result);
        }
    }
}
