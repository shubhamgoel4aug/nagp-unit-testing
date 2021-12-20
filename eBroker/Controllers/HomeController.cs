using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traders.Models;
using Traders.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Codes;

namespace eBroker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ITraderOperations _traderOperations;

        public HomeController(ITraderOperations traderOperations)
        {
            _traderOperations = traderOperations;
        }

        [Route("GetEquities")]
        [HttpGet]
        public List<Equity> GetEquities()
        {
            return _traderOperations.GetEquities();
        }

        [Route("GetTraders")]
        [HttpGet]
        public List<Trader> GetTraders()
        {
            return _traderOperations.GetTraders();
        }

        [Route("GetEquity/{Id}")]
        [HttpGet]
        public Equity GetEquity(String Id)
        {
            int EquityId;
            try
            {
                EquityId = Convert.ToInt32(Id);
            }
            catch(FormatException)
            {
                return new Equity() { Id = 0, Name = "Error:" + ErrorCodes.EquityIdInvalid, Price = 0 };
            }
            if (EquityId <= 0)
                return new Equity() { Id = 0, Name = "Error:" + ErrorCodes.EquityIdNegativeOr0, Price = 0 };
            return _traderOperations.GetEquity(EquityId);
        }

        [Route("GetTrader/{Id}")]
        [HttpGet]
        public Trader GetTrader(String Id)
        {
            int TraderId;
            try
            {
                TraderId = Convert.ToInt32(Id);
            }
            catch (FormatException)
            {
                return new Trader() { Id = 0, Name = "Error:" + ErrorCodes.TraderIdInvalid, Funds = 0, Holdings = "" };
            }
            if (TraderId <= 0)
                return new Trader() { Id = 0, Name = "Error:" + ErrorCodes.TraderIdNegativeOr0, Funds = 0, Holdings = "" };
            return _traderOperations.GetTrader(TraderId);
        }

        [Route("AddFunds")]
        [HttpPost]
        public String AddFunds(JObject data)
        {
            int TraderId = 0;
            double Amount;
            try
            {
                TraderId = Convert.ToInt32(data.GetValue("TraderId").ToString());
                Amount = Convert.ToDouble(data.GetValue("Amount").ToString());
            }
            catch(FormatException)
            {
                return "Error:" + ErrorCodes.InvalidData;
            }
            if (TraderId <= 0)
                return "Error:" + ErrorCodes.TraderIdInvalid;
            if (Amount <= 0)
                return "Error:" + ErrorCodes.FundsAdditionNegativeOr0;
            return _traderOperations.AddFunds(TraderId, Amount);
        }

        [Route("BuyEquity")]
        [HttpPost]
        public String BuyEquity(JObject data)
        {
            int TraderId;
            int EquityId;
            int Units;
            try
            {
                TraderId = Convert.ToInt32(data.GetValue("TraderId").ToString());
                EquityId = Convert.ToInt32(data.GetValue("EquityId").ToString());
                Units = Convert.ToInt32(data.GetValue("Units").ToString());
            }
            catch(FormatException)
            {
                return "Error:" + ErrorCodes.InvalidData;
            }
            if (TraderId <= 0)
                return "Error:" + ErrorCodes.TraderIdInvalid;
            if (EquityId <= 0)
                return "Error:" + ErrorCodes.EquityIdInvalid;
            if (Units <= 0)
                return "Error:" + ErrorCodes.EquityUnitsNegativeOr0;
            return _traderOperations.BuyEquity(TraderId, EquityId, Units);
        }

        [Route("SellEquity")]
        [HttpPost]
        public String SellEquity(JObject data)
        {
            int TraderId;
            int EquityId;
            int Units;
            try
            {
                TraderId = Convert.ToInt32(data.GetValue("TraderId").ToString());
                EquityId = Convert.ToInt32(data.GetValue("EquityId").ToString());
                Units = Convert.ToInt32(data.GetValue("Units").ToString());
            }
            catch (FormatException)
            {
                return "Error:" + ErrorCodes.InvalidData;
            }
            if (TraderId <= 0)
                return "Error:" + ErrorCodes.TraderIdInvalid;
            if (EquityId <= 0)
                return "Error:" + ErrorCodes.EquityIdInvalid;
            if (Units <= 0)
                return "Error:" + ErrorCodes.EquityUnitsNegativeOr0;
            return _traderOperations.SellEquity(TraderId, EquityId, Units);
        }
    }
}
