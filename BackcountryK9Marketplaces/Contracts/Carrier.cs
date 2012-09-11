using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackcountryK9Marketplaces.Contracts
{
    /// <summary>
    /// Class that represents the carrier types.
    /// </summary>
    public class Carrier
    {
        public const string USPSCarrierCode = "USPS";
        public const string FedExCarrierCode = "FedEx";
        public const string UPSCarrierCode = "UPS";

        private string _carrier = string.Empty;
        private string _carrierName = string.Empty;

        public string CarrierCode
        { get { return _carrier; } }

        public string CarrierName
        { get { return _carrierName; } }

        /// <summary>
        /// Hides the default constructor.
        /// </summary>
        private Carrier()
        { }

        /// <summary>
        /// public constructor that accepts a carrier code.
        /// </summary>
        /// <param name="carrierCode">A carrier code.</param>
        public Carrier(string carrierCode)
        {
            switch (carrierCode)
            {
                case (USPSCarrierCode):
                    _carrier = USPSCarrierCode;
                    _carrierName = "United States Postal Service";
                    break;
                case (FedExCarrierCode):
                    _carrier = FedExCarrierCode;
                    _carrierName = "FedEx";
                    break;
                case (UPSCarrierCode):
                    _carrier = UPSCarrierCode;
                    _carrierName = "United Parcel Service";
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Must specify a valid carrier code such as {0}, {1}, {2}", 
                        USPSCarrierCode, UPSCarrierCode, FedExCarrierCode));
            }
        }
    }
}
