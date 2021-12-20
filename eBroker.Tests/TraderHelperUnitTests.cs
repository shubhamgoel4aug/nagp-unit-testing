using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traders.Operations;
using Xunit;

namespace eBroker.Tests
{
    public class TraderHelperUnitTests
    {
        [Fact]
        public void NormalizeFunds_Returns_SameAmount_If_AmountLessThanOrEqual100000()
        {
            Assert.Equal(100000, TraderHelper.NormalizeFunds(100000));
        }

        [Fact]
        public void NormalizeFunds_Returns_AmountAfterReducingCharges_If_AmountGreaterThan100000()
        {
            double amount = 100001;
            Assert.Equal(amount - (amount * 0.05 / 100), TraderHelper.NormalizeFunds(amount));
        }
    }
}
