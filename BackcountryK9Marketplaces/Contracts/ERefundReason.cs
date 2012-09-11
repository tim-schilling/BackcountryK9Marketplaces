using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Types of refunds. Some may not be able to be used with other marketplaces.
    /// 0-15 : Specifically Amazon.
    /// </summary>
    public enum ERefundReason
    {
        NoInventory = 0,
        CustomerReturn = 1,
        GeneralAdjustment = 2,
        CouldNotShip = 3,
        DifferentItem = 4,
        Abandoned = 5,
        CustomerCancel = 6,
        PriceError = 7,
        ProductOutofStock = 8,
        CustomerAddressIncorrect = 9,
        Exchange = 10,
        Other = 11,
        CarrierCreditDecision = 12,
        RiskAssessmentInformationNotValid = 13,
        CarrierCoverageFailure = 14,
        TransactionRecord = 15
    }
}
