using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBrokerDB.Models;

namespace eBrokerDBRepository.Interfaces
{
    public interface ITraderRepository
    {
        Trader GetTrader(int Id);
        List<Trader> GetTraders();
        Equity GetEquity(int Id);
        List<Equity> GetEquities();
        bool AddFunds(int TraderId, double Amount);
        bool BuyEquity(int TraderId, int EquityId, int Units);
        bool SellEquity(int TraderId, int EquityId, int Units, double AmountWithoutBrokerage);
    }
}
