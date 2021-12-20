using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traders.Interfaces;
using Traders.Models;
using eBrokerDBRepository.Interfaces;
using AutoMapper;
using Codes;

namespace Traders.Operations
{
    public class TraderOperations : ITraderOperations
    {
        private readonly ITraderRepository _traderRepository;
        private readonly IMapper _mapper;
        private readonly IWrapper _wrapper;

        public TraderOperations(IMapper mapper, ITraderRepository traderRepository, IWrapper wrapper)
        {
            _traderRepository = traderRepository;
            _mapper = mapper;
            _wrapper = wrapper;
        }

        public String AddFunds(int TraderId, double Amount)
        {
            Trader trader = _mapper.Map<Trader>(_traderRepository.GetTrader(TraderId));

            if (trader.Name.Contains("Error"))
                return "Error:" + ErrorCodes.TraderNotFound;

            if (Amount <= 0)
                return "Error:" + ErrorCodes.FundsAdditionNegativeOr0;

            if (_traderRepository.AddFunds(TraderId, _wrapper.NormalizeFunds(Amount)))
                return "Success:" + SuccessCodes.FundsAdditionSuccess;

            return "Error:" + ErrorCodes.FundsAdditionFailure;
        }

        public String BuyEquity(int TraderId, int EquityId, int Units)
        {
            if (!_wrapper.CheckBuySellTime())
                return "Error:" + ErrorCodes.TradeOutsideTimings;

            Trader trader = _mapper.Map<Trader>(_traderRepository.GetTrader(TraderId));
            Equity equity = _mapper.Map<Equity>(_traderRepository.GetEquity(EquityId));

            if (trader.Name.Contains("Error"))
                return "Error:" + ErrorCodes.TraderNotFound;

            if (equity.Name.Contains("Error"))
                return "Error:" + ErrorCodes.EquityNotFound;

            if (Units <= 0)
                return "Error:" + ErrorCodes.EquityUnitsNegativeOr0;

            if (equity.Price * Units > trader.Funds)
                return "Error:" + ErrorCodes.EquityNotEnoughFunds;

            if (_traderRepository.BuyEquity(TraderId, EquityId, Units))
                return "Success:" + SuccessCodes.EquityBuySuccess;

            return "Error:" + ErrorCodes.EquityBuyFailed;
        }

        public String SellEquity(int TraderId, int EquityId, int Units)
        {
            if (!_wrapper.CheckBuySellTime())
                return "Error:" + ErrorCodes.TradeOutsideTimings;

            Trader trader = _mapper.Map<Trader>(_traderRepository.GetTrader(TraderId));
            Equity equity = _mapper.Map<Equity>(_traderRepository.GetEquity(EquityId));

            if (trader.Name.Contains("Error"))
                return "Error:" + ErrorCodes.TraderNotFound;

            if (equity.Name.Contains("Error"))
                return "Error:" + ErrorCodes.EquityNotFound;

            if (Units <= 0)
                return "Error:" + ErrorCodes.EquityUnitsNegativeOr0;

            Dictionary<int, int> holdings = new Dictionary<int, int>();
            bool holds_equity = false;
            bool holds_quantity = false;
            foreach (String s in trader.Holdings.Split(";"))
            {
                int[] h = s.Split(",").Select(x => Convert.ToInt32(x)).ToArray();
                if (h[0] == EquityId)
                {
                    holds_equity = true;
                    if (h[1] >= Units)
                        holds_quantity = true;
                    break;
                }                    
            }

            if (!holds_equity)
                return "Error:" + ErrorCodes.EquityNotHeldByTrader;

            if (!holds_quantity)
                return "Error:" + ErrorCodes.EquityQuantityNotHeldByTrader;

            if (_traderRepository.SellEquity(TraderId, EquityId, Units, _wrapper.ReduceBrokerage(equity.Price * Units)))
                return "Success:" + SuccessCodes.EquitySellSuccess;

            return "Error:" + ErrorCodes.EquitySellFailed;
        }

        public List<Equity> GetEquities()
        {
            return _mapper.Map<List<Equity>>(_traderRepository.GetEquities());
        }

        public Equity GetEquity(int Id)
        {
            return _mapper.Map<Equity>(_traderRepository.GetEquity(Id));
        }

        public Trader GetTrader(int Id)
        {
            return _mapper.Map<Trader>(_traderRepository.GetTrader(Id));
        }

        public List<Trader> GetTraders()
        {
            return _mapper.Map<List<Trader>>(_traderRepository.GetTraders());
        }
    }
}
