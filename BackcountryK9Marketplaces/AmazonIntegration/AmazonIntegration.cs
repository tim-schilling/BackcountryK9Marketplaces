using BackcountryK9Marketplaces;
using BackcountryK9Marketplaces.Properties;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;

namespace BackcountryK9Marketplaces.AmazonIntegration
{
    /// <summary>
    /// The implementation for the Amazon integration
    /// </summary>
    public class AmazonIntegration : IMarketplaceIntegration
    {
        /// <summary>
        /// The local instance of the amazon client library service client.
        /// </summary>
        private MarketplaceWebServiceClient _AmazonClient;
        /// <summary>
        /// The local instance of the amazon orders client library service client.
        /// </summary>
        private MarketplaceWebServiceOrdersClient _AmazonOrdersClient;

        #region Converting calls to Settings to simple property accessors
        private string _ApplicationName
        {
            get { return Settings.Default.AWSApplicationName; }
        }
        private string _ApplicationVersion
        {
            get { return Settings.Default.AWSApplicationVersion;}
        }
        private string _MerchantId
        {
            get { return Settings.Default.AWSMerchantId;}
        }
        private string _MarketplaceId
        {
            get { return Settings.Default.AWSMarketplaceId;}
        }
        private string _AccessKeyId
        {
            get { return Settings.Default.AWSAccessKeyId;}
        }
        private string _SecretAccessKey
        {
            get { return Settings.Default.AWSSecretAccessKey;}
        }
        private string _TemporaryFileDirectory
        {
            get { return Settings.Default.TempFileDirectory; }
        }
        #endregion

        /// <summary>
        /// Default public contructor. All properties are set via the config file
        /// </summary>
        public AmazonIntegration()
        {
            // Verify that the settings in the config file are setup correctly.
            if (string.IsNullOrWhiteSpace(_AccessKeyId))
                throw new InvalidOperationException("AWSAccessKeyId setting in the config file can't be whitespace, blank or null");
            if (string.IsNullOrWhiteSpace(_SecretAccessKey))
                throw new InvalidOperationException("AWSSecretAccessKey setting in the config file can't be whitespace, blank or null");
            if (string.IsNullOrWhiteSpace(_ApplicationName))
                throw new InvalidOperationException("AWSApplicationName setting in the config file can't be whitespace, blank or null");
            if (string.IsNullOrWhiteSpace(_ApplicationVersion))
                throw new InvalidOperationException("AWSApplicationVersion setting in the config file can't be whitespace, blank or null");
            if (string.IsNullOrWhiteSpace(_MerchantId))
                throw new InvalidOperationException("AWSMerchantId setting in the config file can't be whitespace, blank or null");
            if (string.IsNullOrWhiteSpace(_MarketplaceId))
                throw new InvalidOperationException("AWSMarketplaceId setting in the config file can't be whitespace, blank or null");
            if (string.IsNullOrWhiteSpace(_TemporaryFileDirectory))
                throw new InvalidOperationException("TempFileDirectory setting in the config file can't be whitespace, blank or null");

            var config = new MarketplaceWebServiceConfig();
            // Set configuration to use US marketplace
            config.ServiceURL = "https://mws.amazonservices.com";
            // Set the HTTP Header for user agent for the application.
            config.SetUserAgentHeader(
                _ApplicationName,
                _ApplicationVersion,
                "C#");
            _AmazonClient = new MarketplaceWebServiceClient(_AccessKeyId, _SecretAccessKey, config);

            // Setup the orders service client
            var ordersConfig = new MarketplaceWebServiceOrdersConfig();
            ordersConfig.ServiceURL = "https://mws.amazonservices.com/Orders/2011-01-01";
            ordersConfig.SetUserAgent(_ApplicationName, _ApplicationVersion);
            _AmazonOrdersClient = new MarketplaceWebServiceOrdersClient(
                _ApplicationName, _ApplicationVersion, _AccessKeyId, _SecretAccessKey, ordersConfig);
        }

        /// <summary>
        /// Updates the marketplace's inventory so that items will have the specified quantity available.
        /// </summary>
        /// <param name="items">Collection of objects that represent the item and the quantity the inventory should be updated to.</param>
        /// <exception cref="AmazonUpdateInventoryExcpetion"></exception>
        public void UpdateMarketplaceInventory(IEnumerable<Contracts.MarketplaceInventoryUpdateItem> items)
        {
            if (items.Count() > 0)
            {
                var inventoryUpdateContent = ConvertToStream(items);
                var submitFeedController = new SubmitFeedController(_AmazonClient, _MerchantId, _MarketplaceId);
                var streamResponse = submitFeedController.SubmitFeedAndGetResponse(inventoryUpdateContent, AmazonFeedType.InventoryUpdate);
                var responseException = ConvertResultToUpdateInventoryException(streamResponse);
                if(responseException != null)
                    throw responseException;
            }
        }

        /// <summary>
        /// Fetches the newly created orders from the marketplaces from a given date.
        /// </summary>
        /// <param name="lastUpdated">The starting created order date for when orders are supposed to be pulled.</param>
        /// <returns>A collection of marketplace orders.</returns>
        public IEnumerable<Contracts.MarketplaceOrder> GetMarketplaceOrders(DateTime createdAfter)
        {
            var marketplaceOrders = new List<Contracts.MarketplaceOrder>();
            
            var listOrdersResponse = _AmazonOrdersClient.ListOrders(CreateListOrdersRequest(createdAfter));
            if (!string.IsNullOrWhiteSpace(listOrdersResponse.ListOrdersResult.NextToken))
                throw new MarketplaceWebServiceException("ListOrders returned a next token. Please restructure the code to continue pulling orders with this token.");
            
            // Convert the orders to the marketplace contracts and get the items.
            for(int i = 0; i < listOrdersResponse.ListOrdersResult.Orders.Order.Count; ++i)
            {
                // The maximum request quota for ListOrderItems is 30 after that it restores at 2 per second. 
                // So if the order's that are being pulled exceed 30, then sleep for 2 seconds each one.
                if(i > 30)
                    System.Threading.Thread.Sleep(2000);
                marketplaceOrders.Add(ConvertOrderResponseToMarketplaceOrder(listOrdersResponse.ListOrdersResult.Orders.Order[i]));
            }
            return marketplaceOrders;
        }

        
        /// <summary>
        /// Updates the tracking status of the orders.
        /// </summary>
        /// <param name="orders">A list of orders to be updated for tracking.</param>
        public void UpdateMarketplaceTracking(IEnumerable<Contracts.MarketplaceOrderFulfillment> orders)
        {
            if (orders.Count(o => o.Marketplace == Contracts.EMarketplace.Amazon) > 0)
            {
                var marketplaceOrderFulfillment = ConvertToStream(orders.Where(o => o.Marketplace == Contracts.EMarketplace.Amazon));
                var submitFeedController = new SubmitFeedController(_AmazonClient, _MerchantId, _MarketplaceId);
                var streamResponse = submitFeedController.SubmitFeedAndGetResponse(marketplaceOrderFulfillment, AmazonFeedType.OrderFulfillment);
                var responseException = ConvertResultToOrderFulfillmentException(streamResponse);
                if (responseException != null)
                    throw responseException;
            }
        }

        /// <summary>
        /// Refunds the list of orders.
        /// </summary>
        /// <param name="orders">Orders that have refunds to be issued to customers.</param>
        public void RefundMarketplaceOrders(IEnumerable<Contracts.MarketplaceOrderRefund> orders)
        {
            if (orders.Count(o => o.Marketplace == Contracts.EMarketplace.Amazon) > 0)
            {
                var marketplaceOrderRefundStream = ConvertToStream(orders.Where(o => o.Marketplace == Contracts.EMarketplace.Amazon));
                var submitFeedController = new SubmitFeedController(_AmazonClient, _MerchantId, _MarketplaceId);
                var streamResponse = submitFeedController.SubmitFeedAndGetResponse(marketplaceOrderRefundStream, AmazonFeedType.OrderRefund);
                var responseException = ConvertResultToOrderRefundException(streamResponse);
                if (responseException != null)
                    throw responseException;
            }
        }

        #region ConvertToStream Methods
        /// <summary>
        /// Converts a list of InventoryUpdateItems to a memory stream.
        /// </summary>
        /// <param name="items">The list of items that are to have their quantities updated</param>
        /// <returns>An open memeory stream. This must be closed or handled properly by callers</returns>
        private MemoryStream ConvertToStream(IEnumerable<Contracts.MarketplaceInventoryUpdateItem> items)
        {
            var stream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(stream);
            XmlDocument doc = AmazonXmlHelper.CreateXmlDocForRequest(Settings.Default.AWSMerchantToken, "Inventory");
            for (var i = 0; i < items.Count(); ++i)
            {
                // Each item will have a message envelope, with the id, op type and then the actual data in the inventory element.
                // see page 43 for an example from Amazon and the XSD. https://images-na.ssl-images-amazon.com/images/G/01/rainier/help/XML_Documentation_Intl._V144967332_.pdf
                var msg = doc.LastChild.AppendChild(doc.CreateElement("Message"));
                msg.AppendChild(doc.CreateElement("MessageID")).InnerText = (i + 1).ToString();
                //msg.AppendChild(doc.CreateElement("OperationType")).InnerText = "Update";
                var inventory = msg.AppendChild(doc.CreateElement("Inventory"));
                inventory.AppendChild(doc.CreateElement("SKU")).InnerText = items.ElementAt(i).SKU;
                inventory.AppendChild(doc.CreateElement("Quantity")).InnerText = items.ElementAt(i).Quantity.ToString();
            }
            doc.WriteTo(xmlWriter);
            xmlWriter.Close();
            // Reset the position to 0 for reading.
            stream.Position = 0;
            // If the size of the stream exceeds 1 GB, then throw an exception.
            if (stream.Length > 1073741824)
            {
                var length = stream.Length;
                stream.Close();
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "The xml to be sent exceeds 1 GB. ({0}). Please reduce the size of this call or re-work the code to create a file and return a FileStream.", length));
            }
            return stream;
        }

        /// <summary>
        /// Converts a list of MarketplaceOrderFulfillments to a memory stream.
        /// </summary>
        /// <param name="orders">The list of orders that are to be filled</param>
        /// <returns>An open memeory stream. This must be closed or handled properly by callers</returns>
        private MemoryStream ConvertToStream(IEnumerable<Contracts.MarketplaceOrderFulfillment> orders)
        {
            var stream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(stream);
            XmlDocument doc = AmazonXmlHelper.CreateXmlDocForRequest(Settings.Default.AWSMerchantToken, "OrderFulfillment");
            for (var i = 0; i < orders.Count(); ++i)
            {
                // Each item will have a message envelope, with the id, op type and then the actual data in the inventory element.
                // see page 43 for an example from Amazon and the XSD. https://images-na.ssl-images-amazon.com/images/G/01/rainier/help/XML_Documentation_Intl._V144967332_.pdf
                var msg = doc.LastChild.AppendChild(doc.CreateElement("Message"));
                msg.AppendChild(doc.CreateElement("MessageID")).InnerText = (i + 1).ToString();
                //msg.AppendChild(doc.CreateElement("OperationType")).InnerText = "Update";
                var orderFulfillment = msg.AppendChild(doc.CreateElement("OrderFulfillment"));
                orderFulfillment.AppendChild(doc.CreateElement("AmazonOrderID")).InnerText = orders.ElementAt(i).MarketplaceOrderId;
                //orderFulfillment.AppendChild(doc.CreateElement("MerchantFulfillmentID")).InnerText = Seller supplied value, not used right now
                // http://msdn.microsoft.com/en-us/library/az4se3k1.aspx#Sortable
                // and https://images-na.ssl-images-amazon.com/images/G/01/rainier/help/XML_Documentation_Intl.pdf on page 65. It'll use local time zones.
                orderFulfillment.AppendChild(doc.CreateElement("FulfillmentDate")).InnerText = orders.ElementAt(i).FulfillmentDate.ToString("s");
                var fulfillmentData = orderFulfillment.AppendChild(doc.CreateElement("FulfillmentData"));
                fulfillmentData.AppendChild(doc.CreateElement("CarrierCode")).InnerText = orders.ElementAt(i).Carrier.CarrierCode;
                fulfillmentData.AppendChild(doc.CreateElement("ShippingMethod")).InnerText = orders.ElementAt(i).ShippingMethod;
                fulfillmentData.AppendChild(doc.CreateElement("ShipperTrackingNumber")).InnerText = orders.ElementAt(i).ShipperTrackingNumber;
                foreach (var orderItem in orders.ElementAt(i).Items)
                {
                    var item = orderFulfillment.AppendChild(doc.CreateElement("Item"));
                    item.AppendChild(doc.CreateElement("AmazonOrderItemCode")).InnerText = orderItem.MarketplaceOrderItemId;
                    item.AppendChild(doc.CreateElement("Quantity")).InnerText = orderItem.Quantity.ToString();
                    //item.AppendChild(doc.CreateElement("MerchantFulfillmentItemID")).InnerText = orderItem.SKU; The SKU is not for this. It's looking for some ID.
                }

            }
            doc.WriteTo(xmlWriter);
            xmlWriter.Close();
            // Reset the position to 0 for reading.
            stream.Position = 0;
            // If the size of the stream exceeds 1 GB, then throw an exception.
            if (stream.Length > 1073741824)
            {
                var length = stream.Length;
                stream.Close();
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "The xml to be sent exceeds 1 GB. ({0}). Please reduce the size of this call or re-work the code to create a file and return a FileStream.", length));
            }
            return stream;
        }
        /// <summary>
        /// Converts a list of MarketplaceOrderFulfillments to a memory stream.
        /// </summary>
        /// <param name="orders">The list of orders that are to be filled</param>
        /// <returns>An open memeory stream. This must be closed or handled properly by callers</returns>
        private Stream ConvertToStream(IEnumerable<Contracts.MarketplaceOrderRefund> orders)
        {
            var stream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(stream);
            XmlDocument doc = AmazonXmlHelper.CreateXmlDocForRequest(Settings.Default.AWSMerchantToken, "OrderAdjustment");
            for (var i = 0; i < orders.Count(); ++i)
            {
                // Each item will have a message envelope, with the id, op type and then the actual data in the inventory element.
                // see page 43 for an example from Amazon and the XSD. https://images-na.ssl-images-amazon.com/images/G/01/rainier/help/XML_Documentation_Intl._V144967332_.pdf
                var msg = doc.LastChild.AppendChild(doc.CreateElement("Message"));
                msg.AppendChild(doc.CreateElement("MessageID")).InnerText = (i + 1).ToString();
                //msg.AppendChild(doc.CreateElement("OperationType")).InnerText = "Update";
                var orderAdjustment = msg.AppendChild(doc.CreateElement("OrderAdjustment"));
                orderAdjustment.AppendChild(doc.CreateElement("AmazonOrderID")).InnerText = orders.ElementAt(i).MarketplaceOrderId;
                foreach (var orderItem in orders.ElementAt(i).Items)
                {
                    var item = orderAdjustment.AppendChild(doc.CreateElement("AdjustedItem"));
                    item.AppendChild(doc.CreateElement("AmazonOrderItemCode")).InnerText = orderItem.MarketplaceOrderItemId;
                    item.AppendChild(doc.CreateElement("AdjustmentReason")).InnerText = AmazonXmlHelper.ConvertToString(orderItem.RefundReason);
                    var priceAdjustment = item.AppendChild(doc.CreateElement("ItemPriceAdjustments"));
                    CreateItemPriceAdjustmentXml(doc, "Principal", orderItem.RefundPrice, priceAdjustment);
                    CreateItemPriceAdjustmentXml(doc, "Shipping", orderItem.RefundShipping, priceAdjustment);
                    CreateItemPriceAdjustmentXml(doc, "Tax", orderItem.RefundTax, priceAdjustment);
                    CreateItemPriceAdjustmentXml(doc, "ShippingTax", orderItem.RefundShippingTax, priceAdjustment);
                    CreateItemPriceAdjustmentXml(doc, "ReturnShipping", orderItem.ReturnShipping, priceAdjustment);
                    // Put the quantity cancelled after the prices
                    item.AppendChild(doc.CreateElement("QuantityCancelled")).InnerText = orderItem.QuantityCancelled.ToString();
                }

            }
            doc.WriteTo(xmlWriter);
            xmlWriter.Close();
            // Reset the position to 0 for reading.
            stream.Position = 0;
            // If the size of the stream exceeds 1 GB, then throw an exception.
            if (stream.Length > 1073741824)
            {
                var length = stream.Length;
                stream.Close();
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "The xml to be sent exceeds 1 GB. ({0}). Please reduce the size of this call or re-work the code to create a file and return a FileStream.", length));
            }
            return stream;
        }
        #endregion

        #region ConvertToException Methods
        /// <summary>
        /// Converts a SubmitFeedResponse for the UpdateInventory flow into an InventoryException if needed.
        /// </summary>
        /// <param name="submitFeedResponse">The response object.</param>
        /// <returns>An exception if an error exists in the response object, else null.</returns>
        private AmazonUpdateInventoryException ConvertResultToUpdateInventoryException(Stream submitFeedResponse)
        {
            using (Stream responseStream = submitFeedResponse)
            {
                // The result may not be an xml document. This will be revealed with testing.
                var doc = new XmlDocument();
                doc.Load(responseStream);

                // Check to see if any of the updates had an error else just return fine.
                var report = doc.SelectSingleNode("/AmazonEnvelope/Message/ProcessingReport");
                var errorResults = report.SelectNodes("Result[ResultCode='Error']");
                if (errorResults.Count > 0)
                {
                    var processingSummary = report.SelectSingleNode("ProcessingSummary");
                    var successCount = int.Parse(processingSummary.SelectSingleNode("MessagesSuccessful").InnerText);
                    var errorCount = int.Parse(processingSummary.SelectSingleNode("MessagesWithError").InnerText);
                    var warningCount = int.Parse(processingSummary.SelectSingleNode("MessagesWithWarning").InnerText);
                    var exception = new AmazonUpdateInventoryException(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                        "{0} Products did not update successfully. {1} Products updated sucessfully. {2} Products updated with warnings.",
                        errorCount,
                        successCount,
                        warningCount));

                    foreach (XmlNode result in errorResults)
                    {
                        exception.ErrorResults.Add(new AmazonUpdateInventoryException.Result()
                        {
                            Code = result.SelectSingleNode("ResultMessageCode").InnerText,
                            Description = result.SelectSingleNode("ResultDescription").InnerText,
                            SKU = result.SelectSingleNode("AdditionalInfo/SKU").InnerText
                        });
                    }
                    return exception;
                }
                else return null;
            }
        }
        /// <summary>
        /// Converts a SubmitFeedResponse for the UpdateOrderFulfillment flow into an OrderFulfillmentException if needed.
        /// </summary>
        /// <param name="submitFeedResponse">The response object.</param>
        /// <returns>An exception if an error exists in the response object, else null.</returns>
        private AmazonOrderFulfillmentException ConvertResultToOrderFulfillmentException(Stream submitFeedResponse)
        {
            using (Stream responseStream = submitFeedResponse)
            {
                // The result may not be an xml document. This will be revealed with testing.
                var doc = new XmlDocument();
                doc.Load(responseStream);

                // Check to see if any of the updates had an error else just return fine.
                var report = doc.SelectSingleNode("/AmazonEnvelope/Message/ProcessingReport");
                var errorResults = report.SelectNodes("Result[ResultCode='Error']");
                if (errorResults.Count > 0)
                {
                    var processingSummary = report.SelectSingleNode("ProcessingSummary");
                    var successCount = int.Parse(processingSummary.SelectSingleNode("MessagesSuccessful").InnerText);
                    var errorCount = int.Parse(processingSummary.SelectSingleNode("MessagesWithError").InnerText);
                    var warningCount = int.Parse(processingSummary.SelectSingleNode("MessagesWithWarning").InnerText);
                    var exception = new AmazonOrderFulfillmentException(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                        "{0} Orders did not update successfully. {1} Orders updated sucessfully. {2} Orders updated with warnings.",
                        errorCount,
                        successCount,
                        warningCount));

                    foreach (XmlNode result in errorResults)
                    {
                        exception.ErrorResults.Add(new AmazonOrderFulfillmentException.Result()
                        {
                            Code = result.SelectSingleNode("ResultMessageCode").InnerText,
                            Description = result.SelectSingleNode("ResultDescription").InnerText,
                        });
                    }
                    return exception;
                }
                else return null;
            }
        }
        /// <summary>
        /// Converts a SubmitFeedResponse for the UpdateOrderFulfillment flow into an OrderFulfillmentException if needed.
        /// </summary>
        /// <param name="submitFeedResponse">The response object.</param>
        /// <returns>An exception if an error exists in the response object, else null.</returns>
        private AmazonOrderRefundException ConvertResultToOrderRefundException(Stream submitFeedResponse)
        {
            using (Stream responseStream = submitFeedResponse)
            {
                // The result may not be an xml document. This will be revealed with testing.
                var doc = new XmlDocument();
                doc.Load(responseStream);

                // Check to see if any of the updates had an error else just return fine.
                var report = doc.SelectSingleNode("/AmazonEnvelope/Message/ProcessingReport");
                var errorResults = report.SelectNodes("Result[ResultCode='Error']");
                if (errorResults.Count > 0)
                {
                    var processingSummary = report.SelectSingleNode("ProcessingSummary");
                    var successCount = int.Parse(processingSummary.SelectSingleNode("MessagesSuccessful").InnerText);
                    var errorCount = int.Parse(processingSummary.SelectSingleNode("MessagesWithError").InnerText);
                    var warningCount = int.Parse(processingSummary.SelectSingleNode("MessagesWithWarning").InnerText);
                    var exception = new AmazonOrderRefundException(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                        "{0} Orders did not update successfully. {1} Orders updated sucessfully. {2} Orders updated with warnings.",
                        errorCount,
                        successCount,
                        warningCount));

                    foreach (XmlNode result in errorResults)
                    {
                        exception.ErrorResults.Add(new AmazonOrderRefundException.Result()
                        {
                            Code = result.SelectSingleNode("ResultMessageCode").InnerText,
                            Description = result.SelectSingleNode("ResultDescription").InnerText,
                        });
                    }
                    return exception;
                }
                else return null;
            }
        }
        #endregion

        #region Orphan Conversion Helpers
        /// <summary>
        /// Converts the response object into MarketplaceOrder object. Also makes the call to get the items for the order.
        /// </summary>
        /// <param name="order">The given amazon order object.</param>
        /// <returns>A MarketplaceOrder object</returns>
        private Contracts.MarketplaceOrder ConvertOrderResponseToMarketplaceOrder(Order order)
        {
            var marketplaceOrder = new Contracts.MarketplaceOrder()
            {
                BuyerShippingAddress1 = order.ShippingAddress.AddressLine1,
                BuyerShippingAddress2 = order.ShippingAddress.AddressLine2,
                BuyerShippingAddress3 = order.ShippingAddress.AddressLine3,
                BuyerShippingCity = order.ShippingAddress.City,
                BuyerShippingState = order.ShippingAddress.StateOrRegion,
                BuyerShippingZip = order.ShippingAddress.PostalCode,
                BuyerShippingName = order.ShippingAddress.Name,
                BuyerShippingPhone = order.ShippingAddress.Phone,
                BuyerEmail = order.BuyerEmail,
                BuyerName = order.BuyerName,
                OrderTotal = decimal.Parse(order.OrderTotal.Amount),
                MarketplaceOrderId = order.AmazonOrderId,
                OrderStatus = order.OrderStatus == OrderStatusEnum.PartiallyShipped
                    ? Contracts.EOrderStatus.PartiallyShipped
                    : Contracts.EOrderStatus.Unshipped,
                Marketplace = Contracts.EMarketplace.Amazon,
                ShipmentServiceLevel = order.ShipmentServiceLevelCategory
            };

            var listItemsResponse = _AmazonOrdersClient.ListOrderItems(CreateListOrderItemsRequest(order));
            if (!string.IsNullOrWhiteSpace(listItemsResponse.ListOrderItemsResult.NextToken))
                throw new MarketplaceWebServiceException("ListOrderItems returned a next token. Please restructure the code to continue pulling order items with this token.");
            // Convert the item responses to marketplace order item objects.
            marketplaceOrder.Items = listItemsResponse.ListOrderItemsResult.OrderItems.OrderItem.Select(item =>
                new Contracts.MarketplaceOrderItem()
                {
                    MarketplaceItemId = item.ASIN,
                    MarketplaceOrderItemId = item.OrderItemId,
                    SKU = item.SellerSKU,
                    Price = decimal.Parse(item.ItemPrice.Amount),
                    ShippingPrice = decimal.Parse(item.ShippingPrice.Amount),
                    Tax = decimal.Parse(item.ItemTax.Amount),
                    ShippingTax = decimal.Parse(item.ShippingTax.Amount),
                    QuantityShipped = Convert.ToInt32(item.QuantityShipped),
                    QuantityUnshipped = Convert.ToInt32(item.QuantityOrdered - item.QuantityShipped)
                }
            );
            return marketplaceOrder;
        }

        /// <summary>
        /// Creates a ListOrderItems request object for a given order.
        /// </summary>
        /// <param name="order">The order in which the items for it are being requested.</param>
        /// <returns>A ListOrderItemsRequest object.</returns>
        private ListOrderItemsRequest CreateListOrderItemsRequest(Order order)
        {
            var listItemsRequest = new ListOrderItemsRequest()
            {
                SellerId = _MerchantId,
                AmazonOrderId = order.AmazonOrderId
            };
            return listItemsRequest;
        }

        /// <summary>
        /// Creates a ListOrdersRequest object with the specified created after date.
        /// </summary>
        /// <param name="createdAfter">The date in which orders should have been created after.</param>
        /// <returns>A list orders request object.</returns>
        private ListOrdersRequest CreateListOrdersRequest(DateTime createdAfter)
        {
            var listOrdersRequest = new ListOrdersRequest()
            {
                SellerId = _MerchantId,
                MarketplaceId = new MarketplaceIdList() { Id = new List<string> { _MarketplaceId } },
                LastUpdatedAfter = createdAfter,
                // Only get those orders that have been paid, but haven't been shipped or only partially shipped.
                OrderStatus = new OrderStatusList()
                {
                    Status = new List<OrderStatusEnum> { OrderStatusEnum.Unshipped, OrderStatusEnum.PartiallyShipped }
                }
            };
            return listOrdersRequest;
        }

        /// <summary>
        /// Creates the xml needed for specific item refund prices.
        /// </summary>
        /// <param name="doc">The xml document</param>
        /// <param name="priceType">The type of refund for the specific price.</param>
        /// <param name="refundAmount">The amount to be refunded.</param>
        /// <param name="priceAdjustment">The element in the document that it should be appended to.</param>
        private static void CreateItemPriceAdjustmentXml(XmlDocument doc, string priceType, 
            decimal refundAmount, XmlNode priceAdjustment)
        {
            var currencyAttrib = doc.CreateAttribute("currency");
            currencyAttrib.Value = "USD";
            var component = priceAdjustment.AppendChild(doc.CreateElement("Component"));
            component.AppendChild(doc.CreateElement("Type")).InnerText = priceType;
            var amount = component.AppendChild(doc.CreateElement("Amount"));
            amount.InnerText = refundAmount.ToString();
            amount.Attributes.Append(currencyAttrib);
        }
        #endregion
    }
}
