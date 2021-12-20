using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traders.Operations
{
    public class TraderHelper
    {
        public static double NormalizeFunds(double Amount)
        {
            if (Amount > 100000)
                return Amount - (Amount * 0.05 / 100);
            else return Amount;
        }

        public static bool CheckBuySellTime()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return false;
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour >= 14)
                return false;
            return true;
        }

        public static double ReduceBrokerage(double Amount)
        {
            double brokerage = Amount * 0.05 / 100;
            if (brokerage > 20)
                return Amount - brokerage;
            return Amount - 20;
        }
    }
}
