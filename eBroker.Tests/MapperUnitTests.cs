using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Traders.Mapper;
using DB = eBrokerDB.Models;
using MD = Traders.Models;

namespace eBroker.Tests
{
    public class MapperUnitTests
    {
        MapperConfiguration mapperConfiguration;

        public MapperUnitTests()
        {
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<TraderMapper>());
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void TraderMapper_Maps_DBEquity_MDEquity()
        {
            DB.Equity db_e = new DB.Equity() { Id = 1, Name = "DB Equity", Price = 1000 };
            IMapper mapper = mapperConfiguration.CreateMapper();
            MD.Equity md_e = mapper.Map<MD.Equity>(db_e);
            Assert.NotNull(md_e);
        }

        [Fact]
        public void TraderMapper_Maps_DBTrader_MDTrader()
        {
            DB.Trader db_t = new DB.Trader() { Id = 1, Name = "DB Equity", Funds = 1000, Holdings = "1,10" };
            IMapper mapper = mapperConfiguration.CreateMapper();
            MD.Trader md_d = mapper.Map<MD.Trader>(db_t);
            Assert.NotNull(md_d);
        }
    }
}
