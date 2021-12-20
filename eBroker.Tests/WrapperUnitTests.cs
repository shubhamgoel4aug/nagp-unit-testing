using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traders.Operations;
using Xunit;

namespace eBroker.Tests
{
    public class WrapperUnitTests
    {
        [Fact]
        public void CheckBuySellTime_Returns_bool()
        {
            Wrapper wrapper = new Wrapper();
            Assert.IsType<bool>(wrapper.CheckBuySellTime());
        }

        [Fact]
        public void NormalizeFunds_Return_double()
        {
            Wrapper wrapper = new Wrapper();
            Assert.IsType<double>(wrapper.NormalizeFunds(1000));
        }

        [Fact]
        public void ReduceBrokerage_Return_double()
        {
            Wrapper wrapper = new Wrapper();
            Assert.IsType<double>(wrapper.ReduceBrokerage(1000));
        }
    }
}
