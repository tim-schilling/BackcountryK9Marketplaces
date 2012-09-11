using BackcountryK9Marketplaces.AmazonIntegration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BackcountryK9Marketplaces.Contracts;
using System.Collections.Generic;
using System.IO;
using MarketplaceWebService.Model;
using System.Xml;
using System.Linq;

namespace BackcountryK9Marketplaces.Tests
{
    
    
    /// <summary>
    ///This is a test class for AmazonIntegrationTest and is intended
    ///to contain all AmazonIntegrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AmazonIntegrationTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// Tests the UpdateMarketplaceInventory method
        /// </summary>
        [TestMethod]
        public void UpdateMarketplaceInventorySuccessfulTest()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            var products = new[] { new MarketplaceInventoryUpdateItem() { Quantity = 10, SKU = "11011" } };
            //integration.UpdateMarketplaceInventory(products);
        }
        /// <summary>
        /// Tests the UpdateMarketplaceInventory method for the exception case but processes one message successfully
        /// </summary>
        [TestMethod]
        public void UpdateMarketplaceInventoryMarketplaceProcessingErrorAndSuccessTest()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            var products = new[] { new MarketplaceInventoryUpdateItem() { Quantity = 1, SKU = "TEST-ITEM" }, new MarketplaceInventoryUpdateItem() { Quantity = 10, SKU = "11011" } };
            try
            {
                //integration.UpdateMarketplaceInventory(products);
                // this should throw an exception.
                //Assert.Fail();
            }
            catch (AmazonUpdateInventoryException e)
            {
                Assert.AreEqual(products.Length - 1, e.ErrorResults.Count, "The number of error products is off");
            }
            catch (Exception e)
            {
                // no other exception should be thrown.
                Assert.Fail();
            }
        }
        /// <summary>
        /// Tests the UpdateMarketplaceInventory method for the exception case
        /// </summary>
        [TestMethod]
        public void UpdateMarketplaceInventoryMarketplaceProcessingErrorTest()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            var products = new[] { new MarketplaceInventoryUpdateItem() { Quantity = 1, SKU = "TEST-ITEM" }, new MarketplaceInventoryUpdateItem() { Quantity = 1, SKU = "TEST-ITEM2" } };
            try
            {
                integration.UpdateMarketplaceInventory(products);
                // this should throw an exception.
                Assert.Fail();
            }
            catch (AmazonUpdateInventoryException e)
            {
                Assert.AreEqual(products.Length, e.ErrorResults.Count, "The number of error products is off");
            }
            catch (Exception e)
            {
                // no other exception should be thrown.
                Assert.Fail();
            }
        }
        /// <summary>
        /// Tests the UpdateMarketplaceInventory method
        /// </summary>
        [TestMethod]
        public void UpdateMarketplaceInventoryMarketplaceFeedSubmissionErrorTest()
        {
            // Should test and cause the amazon library to throw an excpetion when processing.
            Assert.Inconclusive();
        }

        /// <summary>
        /// Tests the GetMarketplaceOrders method and pulls data for the past 3 days.
        /// </summary>
        [TestMethod]
        public void GetMarketplaceOrdersTestForPast3Days()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            // get the orders for the past 3 days
            var createdAfter = DateTime.Now.AddDays(-3);
            IEnumerable<MarketplaceOrder> orders = integration.GetMarketplaceOrders(createdAfter);
            Assert.IsTrue(orders.Count() > 0);
            foreach (var order in orders)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(order.BuyerName));
                Assert.IsTrue(!string.IsNullOrWhiteSpace(order.MarketplaceOrderId));
                Assert.AreEqual(EMarketplace.Amazon, order.Marketplace);
                Assert.AreNotEqual(EOrderStatus.Shipped, order.OrderStatus);
                foreach (var item in order.Items)
                {
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.MarketplaceItemId));
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.MarketplaceOrderItemId));
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.SKU));
                }
            }
        }

        /// <summary>
        /// Tests the GetMarketplaceOrders method, but *shouldn't* pull any orders.
        /// </summary>
        [TestMethod]
        public void GetMarketplaceOrdersTestReturnsNone()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            var createdAfter = DateTime.Now.AddMinutes(-2);
            IEnumerable<MarketplaceOrder> orders = integration.GetMarketplaceOrders(createdAfter);
            Assert.AreEqual(0, orders.Count());
        }


        /// <summary>
        /// Tests the UpdateMarketplaceTracking method
        /// </summary>
        [TestMethod]
        public void UpdateMarketplaceTrackingSuccessfulTest()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            var order = integration.GetMarketplaceOrders(DateTime.Now.AddDays(-1)).FirstOrDefault();
            var orders = new[] { 
                new MarketplaceOrderFulfillment
                {
                    Carrier = new Carrier(Carrier.FedExCarrierCode),
                    FulfillmentDate = DateTime.Now,
                    Marketplace = EMarketplace.Amazon,
                    MarketplaceOrderId = order.MarketplaceOrderId,
                    ShippingMethod = "Express Saver",
                    ShipperTrackingNumber= "425921896955",
                    Items = new List<MarketplaceOrderFulfillmentItem>{ 
	                    new MarketplaceOrderFulfillmentItem
	                    {
		                    MarketplaceOrderItemId = order.Items.First().MarketplaceOrderItemId,
		                    Quantity = order.Items.First().QuantityUnshipped
	                    }
                    }
                }
            };
            try
            {
                //integration.UpdateMarketplaceTracking(orders);
                // this should throw an exception.
            }
            catch (AmazonOrderFulfillmentException e)
            {
                Assert.Fail();
            }
            catch (Exception e)
            {
                // no other exception should be thrown.
                Assert.Fail();
            }
        }
        /// <summary>
        /// Tests the UpdateMarketplaceTracking method for the exception case
        /// </summary>
        [TestMethod]
        public void UpdateMarketplaceTrackingMarketplaceProcessingErrorTest()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            var orders = new[] { 
                new MarketplaceOrderFulfillment
                {
                    Carrier = new Carrier(Carrier.FedExCarrierCode),
                    FulfillmentDate = DateTime.Now,
                    Marketplace = EMarketplace.Amazon,
                    MarketplaceOrderId = "1234567",
                    ShippingMethod = "Ground",
                    ShipperTrackingNumber= "test tracking number",
                    Items = new List<MarketplaceOrderFulfillmentItem>{ 
                        new MarketplaceOrderFulfillmentItem
                        {
                            MarketplaceOrderItemId = "123467",
                            Quantity = 5
                        },
                        new MarketplaceOrderFulfillmentItem
                        {
                            MarketplaceOrderItemId = "123468",
                            Quantity = 5
                        }
                    }
                },
                new MarketplaceOrderFulfillment
                {
                    Carrier = new Carrier(Carrier.UPSCarrierCode),
                    FulfillmentDate = DateTime.Now,
                    Marketplace = EMarketplace.Amazon,
                    MarketplaceOrderId = "1234568",
                    ShippingMethod = "Ground",
                    ShipperTrackingNumber= "test tracking number",
                    Items = new List<MarketplaceOrderFulfillmentItem>{ 
                        new MarketplaceOrderFulfillmentItem
                        {
                            MarketplaceOrderItemId = "123467",
                            Quantity = 5
                        },
                        new MarketplaceOrderFulfillmentItem
                        {
                            MarketplaceOrderItemId = "123468",
                            Quantity = 5
                        }
                    }
                },
                new MarketplaceOrderFulfillment
                {
                    Carrier = new Carrier(Carrier.USPSCarrierCode),
                    FulfillmentDate = DateTime.Now,
                    Marketplace = EMarketplace.Amazon,
                    MarketplaceOrderId = "1234569",
                    ShippingMethod = "Ground",
                    ShipperTrackingNumber= "test tracking number",
                    Items = new List<MarketplaceOrderFulfillmentItem>{ 
                        new MarketplaceOrderFulfillmentItem
                        {
                            MarketplaceOrderItemId = "123467",
                            Quantity = 5
                        },
                        new MarketplaceOrderFulfillmentItem
                        {
                            MarketplaceOrderItemId = "123468",
                            Quantity = 5
                        }
                    }
                }
            };
            try
            {
                integration.UpdateMarketplaceTracking(orders);
                // this should throw an exception.
                Assert.Fail();
            }
            catch (AmazonOrderFulfillmentException e)
            {
                Assert.IsTrue(orders.Length <= e.ErrorResults.Count, "The number of errors is less than the orders");
            }
            catch (Exception e)
            {
                // no other exception should be thrown.
                Assert.Fail();
            }
        }

        /// <summary>
        /// Tests the RefundMarketplaceOrders method for the exception case
        /// </summary>
        [TestMethod]
        public void RefundMarketplaceOrdersProcessingErrorTest()
        {
            IMarketplaceIntegration integration = new AmazonIntegration.AmazonIntegration();
            var orders = new[] { 
                new MarketplaceOrderRefund
                {
                    Marketplace = EMarketplace.Amazon,
                    MarketplaceOrderId = "1234567",
                    Items = new List<MarketplaceOrderRefundItem>{ 
                        new MarketplaceOrderRefundItem
                        {
                            MarketplaceOrderItemId = "123467",
                            RefundPrice = 5.5m,
                            QuantityCancelled = 5
                        },
                        new MarketplaceOrderRefundItem
                        {
                            MarketplaceOrderItemId = "123468",
                            RefundPrice = 3.5m,
                            QuantityCancelled = 5
                        }
                    }
                },
                new MarketplaceOrderRefund
                {
                    Marketplace = EMarketplace.Amazon,
                    MarketplaceOrderId = "1234568",
                    Items = new List<MarketplaceOrderRefundItem>{ 
                        new MarketplaceOrderRefundItem
                        {
                            MarketplaceOrderItemId = "1234678",
                            RefundPrice = 5.5m,
                            QuantityCancelled = 5
                        },
                        new MarketplaceOrderRefundItem
                        {
                            MarketplaceOrderItemId = "1234689",
                            RefundPrice = 3.5m,
                            QuantityCancelled = 5
                        }
                    }
                }
            };
            try
            {
                integration.RefundMarketplaceOrders(orders);
                // this should throw an exception.
                Assert.Fail();
            }
            catch (AmazonOrderRefundException e)
            {
                Assert.IsTrue(orders.Length <= e.ErrorResults.Count, "The number of errors is less than the orders");
            }
            catch (Exception e)
            {
                // no other exception should be thrown.
                Assert.Fail();
            }
        }
    }
}
