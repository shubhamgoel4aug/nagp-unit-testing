using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traders.Interfaces;

namespace Traders.Operations
{
    public class Wrapper : IWrapper
    {
        public bool CheckBuySellTime()
        {
            return TraderHelper.CheckBuySellTime();
        }

        public double NormalizeFunds(double Amount)
        {
            return TraderHelper.NormalizeFunds(Amount);
        }

        public double ReduceBrokerage(double Amount)
        {
            return TraderHelper.ReduceBrokerage(Amount);
        }
    }
}
