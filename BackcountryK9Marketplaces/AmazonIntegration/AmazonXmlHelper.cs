using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackcountryK9Marketplaces.Contracts;
using System.Xml;

namespace BackcountryK9Marketplaces.AmazonIntegration
{
    /// <summary>
    /// This class provides a set of utility functions to help with XML translations for the Amazon integration.
    /// </summary>
    internal static class AmazonXmlHelper
    {
        /// <summary>
        /// Converts an enumeration into a string.
        /// </summary>
        /// <param name="refundReason">The enumeration type.</param>
        /// <returns>The string representation</returns>
        public static string ConvertToString(ERefundReason refundReason)
        {
            switch (refundReason)
            {
                case ERefundReason.NoInventory: 
                    return "NoInventory";
                case ERefundReason.CustomerReturn:
                    return "CustomerReturn";
                case ERefundReason.GeneralAdjustment:
                    return "GeneralAdjustment";
                case ERefundReason.CouldNotShip:
                    return "CouldNotShip";
                case ERefundReason.DifferentItem:
                    return "DifferentItem";
                case ERefundReason.Abandoned:
                    return "Abandoned";
                case ERefundReason.CustomerCancel:
                    return "CustomerCancel";
                case ERefundReason.PriceError:
                    return "PriceError";
                case ERefundReason.ProductOutofStock:
                    return "ProductOutofStock";
                case ERefundReason.CustomerAddressIncorrect:
                    return "CustomerAddressIncorrect";
                case ERefundReason.Exchange:
                    return "Exchange";
                case ERefundReason.Other:
                    return "Other";
                case ERefundReason.CarrierCreditDecision:
                    return "CarrierCreditDecision";
                case ERefundReason.RiskAssessmentInformationNotValid:
                    return "RiskAssessmentInformationNotValid";
                case ERefundReason.CarrierCoverageFailure:
                    return "CarrierCoverageFailure";
                case ERefundReason.TransactionRecord:
                    return "TransactionRecord";
                default:
                    throw new InvalidOperationException("Enumeration type was not able to be converted for amazon.");
            }
        }

        /// <summary>
        /// Creates the XmlDocument for feed requests.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns></returns>
        public static XmlDocument CreateXmlDocForRequest(string merchantId, string messageType)
        {
            XmlDocument doc = new XmlDocument();
            // Set the envelope specifically. The attribute for xsi:noNamespaceSchemaLocation is a hack, but it works and shouldn't have to change in the future.
            doc.LoadXml("<?xml version='1.0' encoding='UTF-8'?><AmazonEnvelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:noNamespaceSchemaLocation='amzn-envelope.xsd'></AmazonEnvelope>");
            // Create the Xml for the InventoryUpdate call.
            var header = doc.LastChild.AppendChild(doc.CreateElement("Header"));
            header.AppendChild(doc.CreateElement("DocumentVersion")).InnerText = "1.01";
            header.AppendChild(doc.CreateElement("MerchantIdentifier")).InnerText = merchantId;
            doc.LastChild.AppendChild(doc.CreateElement("MessageType")).InnerText = messageType;
            return doc;
        }
    }
}
