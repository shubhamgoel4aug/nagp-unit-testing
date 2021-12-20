using Microsoft.AspNetCore.Mvc.Testing;
using System;
using Xunit;
using eBroker;
using System.Threading.Tasks;
using Traders.Operations;
using Traders.Models;
using Traders.Interfaces;
using Moq;
using eBroker.Controllers;
using System.Collections.Generic;
using Codes;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eBroker.Tests
{
    public class eBrokerUnitTest
    {
        Mock<ITraderOperations> traderOperations;
        HomeController homeController;

        List<Equity> equities = new List<Equity>()
        {
            new Equity() { Id = 1, Name = "Equity1", Price = 100 },
            new Equity() { Id = 2, Name = "Equity2", Price = 200 },
            new Equity() { Id = 3, Name = "Equity3", Price = 300 }
        };

        List<Trader> traders = new List<Trader>()
        {
            new Trader() { Id = 1, Name = "Trader1", Funds = 1000, Holdings = "1,5;2,10" },
            new Trader() { Id = 2, Name = "Trader2", Funds = 2000, Holdings = "2,5;3,10" },
            new Trader() { Id = 3, Name = "Trader3", Funds = 3000, Holdings = "1,5;3,10" }
        };

        public eBrokerUnitTest()
        {
            traderOperations = new Mock<ITraderOperations>();
            homeController = new HomeController(traderOperations.Object);
        }

        [Fact]
        public void GetsAllEquities()
        {
            traderOperations.Setup(x => x.GetEquities()).Returns(equities);

            bool result = homeController.GetEquities().Count > 0;

            Assert.True(result);
        }

        [Fact]
        public void GetsAllTraders()
        {
            traderOperations.Setup(x => x.GetTraders()).Returns(traders);

            bool result = homeController.GetTraders().Count > 0;

            Assert.True(result);
        }

        [Fact]
        public void GetsSingleEquity_Success()
        {
            traderOperations.Setup(x => x.GetEquity(It.IsAny<int>())).Returns(equities[0]);

            bool result = homeController.GetEquity("1").Name.Equals("Equity1");

            Assert.True(result);
        }

        [Fact]
        public void GetsSingleEquity_Fail_EquityIdInvalid()
        {
            traderOperations.Setup(x => x.GetEquity(It.IsAny<int>())).Returns(equities[0]);

            bool result = homeController.GetEquity("s").Name.Equals("Error:" + ErrorCodes.EquityIdInvalid);

            Assert.True(result);
        }

        [Fact]
        public void GetsSingleEquity_Fail_EquityIdNegativeOr0()
        {
            traderOperations.Setup(x => x.GetEquity(It.IsAny<int>())).Returns(equities[0]);

            bool result = homeController.GetEquity("0").Name.Equals("Error:" + ErrorCodes.EquityIdNegativeOr0);

            Assert.True(result);
        }

        [Fact]
        public void GetsSingleTrader_Success()
        {
            traderOperations.Setup(x => x.GetTrader(It.IsAny<int>())).Returns(traders[0]);

            bool result = homeController.GetTrader("1").Name.Equals("Trader1");

            Assert.True(result);
        }

        [Fact]
        public void GetsSingleTrader_Fail_TraderIdInvalid()
        {
            traderOperations.Setup(x => x.GetTrader(It.IsAny<int>())).Returns(traders[0]);

            bool result = homeController.GetTrader("s").Name.Equals("Error:" + ErrorCodes.TraderIdInvalid);

            Assert.True(result);
        }

        [Fact]
        public void GetsSingleTrader_Fail_TraderIdNegativeOr0()
        {
            traderOperations.Setup(x => x.GetTrader(It.IsAny<int>())).Returns(traders[0]);

            bool result = homeController.GetTrader("0").Name.Equals("Error:" + ErrorCodes.TraderIdNegativeOr0);

            Assert.True(result);
        }

        [Fact]
        public void AddFunds_Success()
        {
            traderOperations.Setup(x => x.AddFunds(It.IsAny<int>(), It.IsAny<double>())).Returns("Success:" + SuccessCodes.FundsAdditionSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"Amount\": 100000 }");

            bool result = homeController.AddFunds(post).Equals("Success:" + SuccessCodes.FundsAdditionSuccess);

            Assert.True(result);
        }

        [Fact]
        public void AddFunds_Fail_InvalidData()
        {
            traderOperations.Setup(x => x.AddFunds(It.IsAny<int>(), It.IsAny<double>())).Returns("Success:" + SuccessCodes.FundsAdditionSuccess);
            JObject post1 = JObject.Parse("{ \"TraderId\": \"s\", \"Amount\": 100000 }");
            JObject post2 = JObject.Parse("{ \"TraderId\": 1, \"Amount\": \"am\" }");
            JObject post3 = JObject.Parse("{ \"TraderId\": \"s\", \"Amount\": \"am\" }");

            bool result1 = homeController.AddFunds(post1).Equals("Error:" + ErrorCodes.InvalidData);
            bool result2 = homeController.AddFunds(post2).Equals("Error:" + ErrorCodes.InvalidData);
            bool result3 = homeController.AddFunds(post3).Equals("Error:" + ErrorCodes.InvalidData);

            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        public void AddFunds_Fail_TraderIdInvalid()
        {
            traderOperations.Setup(x => x.AddFunds(It.IsAny<int>(), It.IsAny<double>())).Returns("Success:" + SuccessCodes.FundsAdditionSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 0, \"Amount\": 100000 }");

            bool result = homeController.AddFunds(post).Equals("Error:" + ErrorCodes.TraderIdInvalid);

            Assert.True(result);
        }

        [Fact]
        public void AddFunds_Fail_FundsAdditionNegativeOr0()
        {
            traderOperations.Setup(x => x.AddFunds(It.IsAny<int>(), It.IsAny<double>())).Returns("Success:" + SuccessCodes.FundsAdditionSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"Amount\": -100000 }");

            bool result = homeController.AddFunds(post).Equals("Error:" + ErrorCodes.FundsAdditionNegativeOr0);

            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_Success()
        {
            traderOperations.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquityBuySuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"EquityId\": 1, \"Units\": 10 }");

            bool result = homeController.BuyEquity(post).Equals("Success:" + SuccessCodes.EquityBuySuccess);

            Assert.True(result);
        }

        [Theory]
        [InlineData("t", "1", "10")]
        [InlineData("t", "e", "10")]
        [InlineData("t", "e", "u")]
        [InlineData("1", "e", "u")]
        [InlineData("1", "1", "u")]
        [InlineData("s", "1", "u")]
        [InlineData("1", "e", "10")]
        public void BuyEquity_Fail_InvalidData(String TraderId, String EquityId, String Units)
        {
            traderOperations.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquityBuySuccess);
            JObject post = JObject.Parse("{ \"TraderId\": \"" + TraderId + "\", \"EquityId\": \"" + EquityId + "\", \"Units\": \"" + Units + "\" }");

            bool result = homeController.BuyEquity(post).Equals("Error:" + ErrorCodes.InvalidData);

            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_Fail_TraderIdInvalid()
        {
            traderOperations.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquityBuySuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 0, \"EquityId\": 1, \"Units\": 10 }");

            bool result = homeController.BuyEquity(post).Equals("Error:" + ErrorCodes.TraderIdInvalid);

            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_Fail_EquityIdInvalid()
        {
            traderOperations.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquityBuySuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"EquityId\": 0, \"Units\": 10 }");

            bool result = homeController.BuyEquity(post).Equals("Error:" + ErrorCodes.EquityIdInvalid);

            Assert.True(result);
        }

        [Fact]
        public void BuyEquity_Fail_EquityUnitsNegativeOr0()
        {
            traderOperations.Setup(x => x.BuyEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquityBuySuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"EquityId\": 1, \"Units\": -10 }");

            bool result = homeController.BuyEquity(post).Equals("Error:" + ErrorCodes.EquityUnitsNegativeOr0);

            Assert.True(result);
        }

        [Fact]
        public void SellEquity_Success()
        {
            traderOperations.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquitySellSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"EquityId\": 1, \"Units\": 10 }");

            bool result = homeController.SellEquity(post).Equals("Success:" + SuccessCodes.EquitySellSuccess);

            Assert.True(result);
        }

        [Theory]
        [InlineData("t", "1", "10")]
        [InlineData("t", "e", "10")]
        [InlineData("t", "e", "u")]
        [InlineData("1", "e", "u")]
        [InlineData("1", "1", "u")]
        [InlineData("s", "1", "u")]
        [InlineData("1", "e", "10")]
        public void SellEquity_Fail_InvalidData(String TraderId, String EquityId, String Units)
        {
            traderOperations.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquitySellSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": \"" + TraderId + "\", \"EquityId\": \"" + EquityId + "\", \"Units\": \"" + Units + "\" }");

            bool result = homeController.SellEquity(post).Equals("Error:" + ErrorCodes.InvalidData);

            Assert.True(result);
        }

        [Fact]
        public void SellEquity_Fail_TraderIdInvalid()
        {
            traderOperations.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquitySellSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 0, \"EquityId\": 1, \"Units\": 10 }");

            bool result = homeController.SellEquity(post).Equals("Error:" + ErrorCodes.TraderIdInvalid);

            Assert.True(result);
        }

        [Fact]
        public void SellEquity_Fail_EquityIdInvalid()
        {
            traderOperations.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquitySellSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"EquityId\": 0, \"Units\": 10 }");

            bool result = homeController.SellEquity(post).Equals("Error:" + ErrorCodes.EquityIdInvalid);

            Assert.True(result);
        }

        [Fact]
        public void SellEquity_Fail_EquityUnitsNegativeOr0()
        {
            traderOperations.Setup(x => x.SellEquity(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns("Success:" + SuccessCodes.EquitySellSuccess);
            JObject post = JObject.Parse("{ \"TraderId\": 1, \"EquityId\": 1, \"Units\": -10 }");

            bool result = homeController.SellEquity(post).Equals("Error:" + ErrorCodes.EquityUnitsNegativeOr0);

            Assert.True(result);
        }

        [Fact]
        public void ConfigureServices_RegistersDependenciesCorrectly()
        {
            Mock<Microsoft.Extensions.Configuration.IConfiguration> configurationStub = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);

            target.ConfigureServices(services);
            services.AddTransient<HomeController>();

            var serviceProvider = services.BuildServiceProvider();
            var controller = serviceProvider.GetService<HomeController>();
            Assert.NotNull(controller);
        }
    }
}
