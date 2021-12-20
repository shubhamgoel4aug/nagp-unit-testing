using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBrokerDB.Models;
using eBrokerDBRepository.Interfaces;
using eBrokerDB;
using Codes;

namespace eBrokerDBRepository.Operations
{
    public class TraderRepository : ITraderRepository
    {
        private readonly EBrokerDBContext _eBrokerDbContext;
        
        public TraderRepository(EBrokerDBContext eBrokerDBContext)
        {
            _eBrokerDbContext = eBrokerDBContext;
        }

        public bool AddFunds(int TraderId, double Amount)
        {
            Trader trader = GetTrader(TraderId);
            trader.Funds += Amount;
            return _eBrokerDbContext.SaveChanges() > 0;
        }

        public bool BuyEquity(int TraderId, int EquityId, int Units)
        {
            Trader trader = GetTrader(TraderId);
            Equity equity = GetEquity(EquityId);
            Dictionary<int, int> holdings = new Dictionary<int, int>();
            String s_holdings = "";
            bool added = false;
            foreach(String s in trader.Holdings.Split(";"))
            {
                int[] h = s.Split(",").Select(x => Convert.ToInt32(x)).ToArray();
                if(h[0] == EquityId)
                {
                    h[1] += Units;
                    added = true;
                }
                holdings.Add(Convert.ToInt32(h[0]), Convert.ToInt32(h[1]));
            }
            if(!added)
            {
                holdings.Add(EquityId, Units);
            }
            foreach(KeyValuePair<int, int> kv in holdings)
                s_holdings += kv.Key + "," + kv.Value + ";";
            trader.Holdings = s_holdings.Trim(';');
            trader.Funds -= equity.Price * Units;
            return _eBrokerDbContext.SaveChanges() > 0;
        }

        public bool SellEquity(int TraderId, int EquityId, int Units, double AmountWithoutBrokerage)
        {
            Trader trader = GetTrader(TraderId);
            Equity equity = GetEquity(EquityId);
            Dictionary<int, int> holdings = new Dictionary<int, int>();
            String s_holdings = "";
            foreach (String s in trader.Holdings.Split(";"))
            {
                int[] h = s.Split(",").Select(x => Convert.ToInt32(x)).ToArray();
                if(h[0] == EquityId)
                    h[1] -= Units;
                holdings.Add(Convert.ToInt32(h[0]), Convert.ToInt32(h[1]));
            }
            foreach (KeyValuePair<int, int> kv in holdings)
                s_holdings += kv.Key + "," + kv.Value + ";";
            trader.Holdings = s_holdings.Trim(';');
            trader.Funds += AmountWithoutBrokerage;
            return _eBrokerDbContext.SaveChanges() > 0;
        }

        public List<Equity> GetEquities()
        {
            return _eBrokerDbContext.Equities.ToList();
        }

        public Equity GetEquity(int Id)
        {
            if (_eBrokerDbContext.Equities.Count(x => x.Id == Id) == 0)
                return new Equity() { Id = 0, Name = "Error:" + ErrorCodes.EquityNotFound, Price = 0 };
            return _eBrokerDbContext.Equities.First(x => x.Id == Id);
        }

        public List<Trader> GetTraders()
        {
            return _eBrokerDbContext.Traders.ToList();
        }

        public Trader GetTrader(int Id)
        {
            if (_eBrokerDbContext.Traders.Count(x => x.Id == Id) == 0)
                return new Trader() { Id = 0, Name = "Error:" + ErrorCodes.TraderNotFound, Funds = 0, Holdings = "" };
            return _eBrokerDbContext.Traders.First(x => x.Id == Id);
        }        
    }
}
