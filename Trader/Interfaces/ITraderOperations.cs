using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traders.Models;

namespace Traders.Interfaces
{
    public interface ITraderOperations
    {
        Trader GetTrader(int Id);
        List<Trader> GetTraders();
        Equity GetEquity(int Id);
        List<Equity> GetEquities();
        String AddFunds(int TraderId, double Amount);
        String BuyEquity(int TraderId, int EquityId, int Units);
        String SellEquity(int TraderId, int EquityId, int Units);
    }
}
