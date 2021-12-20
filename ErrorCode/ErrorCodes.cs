using System;

namespace Codes
{
    public class ErrorCodes
    {
        public const int TraderNotFound = 111;
        public const int EquityNotFound = 121;
        public const int EquityUnitsNegativeOr0 = 122;
        public const int EquityBuyFailed = 124;
        public const int EquityNotEnoughFunds = 125;
        public const int EquitySellFailed = 126;
        public const int EquityNotHeldByTrader = 128;
        public const int EquityQuantityNotHeldByTrader = 129;
        public const int EquityIdInvalid = 1210;
        public const int EquityIdNegativeOr0 = 1211;
        public const int FundsAdditionFailure = 131;
        public const int FundsAdditionNegativeOr0 = 133;
        public const int TradeOutsideTimings = 141;
        public const int TraderIdInvalid = 142;
        public const int TraderIdNegativeOr0 = 143;
        public const int InvalidData = 151;
    }
}
