/******************************************************************************* 
 *  Copyright 2008-2012 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  
 *  You may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 *  specific language governing permissions and limitations under the License.
 * ***************************************************************************** 
 * 
 *  Marketplace Web Service Orders CSharp Library
 *  API Version: 2011-01-01
 * 
 */


using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;



namespace MarketplaceWebServiceOrders.Samples
{

    /// <summary>
    /// List Order Items  Samples
    /// </summary>
    public class ListOrderItemsSample
    {
    
                                     
        /// <summary>
        /// This operation can be used to list the items of the order indicated by the
        /// given order id (only a single Amazon order id is allowed).
        /// 
        /// </summary>
        /// <param name="service">Instance of MarketplaceWebServiceOrders service</param>
        /// <param name="request">ListOrderItemsRequest request</param>
        public static void InvokeListOrderItems(MarketplaceWebServiceOrders service, ListOrderItemsRequest request)
        {
            try 
            {
                ListOrderItemsResponse response = service.ListOrderItems(request);
                
                
                Console.WriteLine ("Service Response");
                Console.WriteLine ("=============================================================================");
                Console.WriteLine ();

                Console.WriteLine("        ListOrderItemsResponse");
                if (response.IsSetListOrderItemsResult())
                {
                    Console.WriteLine("            ListOrderItemsResult");
                    ListOrderItemsResult  listOrderItemsResult = response.ListOrderItemsResult;
                    if (listOrderItemsResult.IsSetNextToken())
                    {
                        Console.WriteLine("                NextToken");
                        Console.WriteLine("                    {0}", listOrderItemsResult.NextToken);
                    }
                    if (listOrderItemsResult.IsSetAmazonOrderId())
                    {
                        Console.WriteLine("                AmazonOrderId");
                        Console.WriteLine("                    {0}", listOrderItemsResult.AmazonOrderId);
                    }
                    if (listOrderItemsResult.IsSetOrderItems())
                    {
                        Console.WriteLine("                OrderItems");
                        OrderItemList  orderItems = listOrderItemsResult.OrderItems;
                        List<OrderItem> orderItemList = orderItems.OrderItem;
                        foreach (OrderItem orderItem in orderItemList)
                        {
                            Console.WriteLine("                    OrderItem");
                            if (orderItem.IsSetASIN())
                            {
                                Console.WriteLine("                        ASIN");
                                Console.WriteLine("                            {0}", orderItem.ASIN);
                            }
                            if (orderItem.IsSetSellerSKU())
                            {
                                Console.WriteLine("                        SellerSKU");
                                Console.WriteLine("                            {0}", orderItem.SellerSKU);
                            }
                            if (orderItem.IsSetOrderItemId())
                            {
                                Console.WriteLine("                        OrderItemId");
                                Console.WriteLine("                            {0}", orderItem.OrderItemId);
                            }
                            if (orderItem.IsSetTitle())
                            {
                                Console.WriteLine("                        Title");
                                Console.WriteLine("                            {0}", orderItem.Title);
                            }
                            if (orderItem.IsSetQuantityOrdered())
                            {
                                Console.WriteLine("                        QuantityOrdered");
                                Console.WriteLine("                            {0}", orderItem.QuantityOrdered);
                            }
                            if (orderItem.IsSetQuantityShipped())
                            {
                                Console.WriteLine("                        QuantityShipped");
                                Console.WriteLine("                            {0}", orderItem.QuantityShipped);
                            }
                            if (orderItem.IsSetItemPrice())
                            {
                                Console.WriteLine("                        ItemPrice");
                                Money  itemPrice = orderItem.ItemPrice;
                                if (itemPrice.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", itemPrice.CurrencyCode);
                                }
                                if (itemPrice.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", itemPrice.Amount);
                                }
                            }
                            if (orderItem.IsSetShippingPrice())
                            {
                                Console.WriteLine("                        ShippingPrice");
                                Money  shippingPrice = orderItem.ShippingPrice;
                                if (shippingPrice.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", shippingPrice.CurrencyCode);
                                }
                                if (shippingPrice.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", shippingPrice.Amount);
                                }
                            }
                            if (orderItem.IsSetGiftWrapPrice())
                            {
                                Console.WriteLine("                        GiftWrapPrice");
                                Money  giftWrapPrice = orderItem.GiftWrapPrice;
                                if (giftWrapPrice.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", giftWrapPrice.CurrencyCode);
                                }
                                if (giftWrapPrice.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", giftWrapPrice.Amount);
                                }
                            }
                            if (orderItem.IsSetItemTax())
                            {
                                Console.WriteLine("                        ItemTax");
                                Money  itemTax = orderItem.ItemTax;
                                if (itemTax.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", itemTax.CurrencyCode);
                                }
                                if (itemTax.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", itemTax.Amount);
                                }
                            }
                            if (orderItem.IsSetShippingTax())
                            {
                                Console.WriteLine("                        ShippingTax");
                                Money  shippingTax = orderItem.ShippingTax;
                                if (shippingTax.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", shippingTax.CurrencyCode);
                                }
                                if (shippingTax.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", shippingTax.Amount);
                                }
                            }
                            if (orderItem.IsSetGiftWrapTax())
                            {
                                Console.WriteLine("                        GiftWrapTax");
                                Money  giftWrapTax = orderItem.GiftWrapTax;
                                if (giftWrapTax.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", giftWrapTax.CurrencyCode);
                                }
                                if (giftWrapTax.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", giftWrapTax.Amount);
                                }
                            }
                            if (orderItem.IsSetShippingDiscount())
                            {
                                Console.WriteLine("                        ShippingDiscount");
                                Money  shippingDiscount = orderItem.ShippingDiscount;
                                if (shippingDiscount.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", shippingDiscount.CurrencyCode);
                                }
                                if (shippingDiscount.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", shippingDiscount.Amount);
                                }
                            }
                            if (orderItem.IsSetPromotionDiscount())
                            {
                                Console.WriteLine("                        PromotionDiscount");
                                Money  promotionDiscount = orderItem.PromotionDiscount;
                                if (promotionDiscount.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", promotionDiscount.CurrencyCode);
                                }
                                if (promotionDiscount.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", promotionDiscount.Amount);
                                }
                            }
                            if (orderItem.IsSetPromotionIds())
                            {
                                Console.WriteLine("                        PromotionIds");
                                PromotionIdList  promotionIds = orderItem.PromotionIds;
                                List<String> promotionIdList  =  promotionIds.PromotionId;
                                foreach (String promotionId in promotionIdList)
                                {
                                    Console.WriteLine("                            PromotionId");
                                    Console.WriteLine("                                {0}", promotionId);
                                }
                            }
                            if (orderItem.IsSetCODFee())
                            {
                                Console.WriteLine("                        CODFee");
                                Money  CODFee = orderItem.CODFee;
                                if (CODFee.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", CODFee.CurrencyCode);
                                }
                                if (CODFee.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", CODFee.Amount);
                                }
                            }
                            if (orderItem.IsSetCODFeeDiscount())
                            {
                                Console.WriteLine("                        CODFeeDiscount");
                                Money  CODFeeDiscount = orderItem.CODFeeDiscount;
                                if (CODFeeDiscount.IsSetCurrencyCode())
                                {
                                    Console.WriteLine("                            CurrencyCode");
                                    Console.WriteLine("                                {0}", CODFeeDiscount.CurrencyCode);
                                }
                                if (CODFeeDiscount.IsSetAmount())
                                {
                                    Console.WriteLine("                            Amount");
                                    Console.WriteLine("                                {0}", CODFeeDiscount.Amount);
                                }
                            }
                            if (orderItem.IsSetGiftMessageText())
                            {
                                Console.WriteLine("                        GiftMessageText");
                                Console.WriteLine("                            {0}", orderItem.GiftMessageText);
                            }
                            if (orderItem.IsSetGiftWrapLevel())
                            {
                                Console.WriteLine("                        GiftWrapLevel");
                                Console.WriteLine("                            {0}", orderItem.GiftWrapLevel);
                            }
                            if (orderItem.IsSetInvoiceData())
                            {
                                Console.WriteLine("                        InvoiceData");
                                InvoiceData  invoiceData = orderItem.InvoiceData;
                                if (invoiceData.IsSetInvoiceRequirement())
                                {
                                    Console.WriteLine("                            InvoiceRequirement");
                                    Console.WriteLine("                                {0}", invoiceData.InvoiceRequirement);
                                }
                                if (invoiceData.IsSetBuyerSelectedInvoiceCategory())
                                {
                                    Console.WriteLine("                            BuyerSelectedInvoiceCategory");
                                    Console.WriteLine("                                {0}", invoiceData.BuyerSelectedInvoiceCategory);
                                }
                                if (invoiceData.IsSetInvoiceTitle())
                                {
                                    Console.WriteLine("                            InvoiceTitle");
                                    Console.WriteLine("                                {0}", invoiceData.InvoiceTitle);
                                }
                                if (invoiceData.IsSetInvoiceInformation())
                                {
                                    Console.WriteLine("                            InvoiceInformation");
                                    Console.WriteLine("                                {0}", invoiceData.InvoiceInformation);
                                }
                            }
                        }
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata  responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }
                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);
                Console.WriteLine();


            } 
            catch (MarketplaceWebServiceOrdersException ex) 
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }
        }
                }
}
