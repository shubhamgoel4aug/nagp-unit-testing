using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traders.Interfaces
{
    public interface IWrapper
    {
        double NormalizeFunds(double Amount);
        bool CheckBuySellTime();
        double ReduceBrokerage(double Amount);
    }
}
