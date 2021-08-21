namespace AutoMockHelper.Samples.NUnit
{
    using AutoMockHelper.Core;
    using AutoMockHelper.Samples.Logic.OrderProcessor;
    using global::NUnit.Framework;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class OrderProcessorTests : AutoMockContext<OrderProcessor>
    {
        [SetUp]
        public void SetUp()
        {
            base.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            base.Cleanup();
        }
        
        //Tests are passing individually, but failing when run together. Need to track this down.

        [Test]
        public void OrderProcessorIsSuccessfullyCreated()
        {
            //Arrange
            //Act
            var instance = this.ClassUnderTest;

            //Assert
            Assert.IsInstanceOf<OrderProcessor>(instance);
        }

        [Test]
        public async Task CreateNewOrderLogsException()
        {
            //Arrange

            //
            //StrictMock() - Enables strict mock so that any interactions that aren't explicitly defined will cause an error when calling Verify (or VerifyAll)
            //
            this.StrictMock<ILogger>();

            this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));
            this.MockFor<ILogger>().Setup(x => x.Error(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}")), It.IsAny<Exception>()));

            this.StrictMock<IOrderRepository>();
            this.MockFor<IOrderRepository>().Setup(x => x.SaveNewOrderAsync(It.IsAny<int>(), It.IsAny<List<OrderItem>>(), It.IsAny<Customer>()))
                .Throws(new ApplicationException("Order Repository is broken!"));

            //Act
            await this.ClassUnderTest.CreateNewOrder(It.IsAny<List<OrderItem>>(), It.IsAny<Customer>());

            //Assert
            //
            //VerifyAll() - Calls Verify() for all mocked objects
            //
            this.VerifyAll();
        }

        [Test]
        public async Task CreateNewOrderRollsBackAndNotifiesWhenTryReserveProductFails()
        {
            //Arrange
            const bool ReserveProductIsSuccessful = false;
            const int NewOrderNumber = TestOrderNumberGeneratorService.DefaultSeed + 1;
            var testSessionId = Guid.NewGuid();
            var testCustomer = new Customer
                           {
                               CustomerId = 42
                           };

            this.StrictMock<ILogger>();
            this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));
            this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"Completed {nameof(OrderProcessor.CreateNewOrder)}"))));

            //
            //Use() - Uses a specific instance of IOrderRepository (test double) instead of using a mock
            //CreateInstance() - Creates an instance of any object, using Mocks for any dependencies (as opposed to using new())
            //
            var inMemoryOrderRepository = this.CreateInstance<InMemoryOrderRepository>();
            this.Use<IOrderRepository>(inMemoryOrderRepository);
            inMemoryOrderRepository.AddNewCustomer(testCustomer);

            //
            //Use() - Uses a specific type instead of using a mock
            //
            this.Use<IOrderNumberGeneratorService, TestOrderNumberGeneratorService>();

            this.StrictMock<IInventoryService>();
            this.MockFor<IInventoryService>().Setup(x => x.OpenSessionAsync())
                .ReturnsAsync(testSessionId);
            this.MockFor<IInventoryService>().Setup(x => x.TryReserveProductsAsync(It.IsAny<List<OrderItem>>()))
                .ReturnsAsync(ReserveProductIsSuccessful);
            this.MockFor<IInventoryService>().Setup(x => x.RollbackSessionAsync(testSessionId))
                .Returns(Task.CompletedTask);

            this.StrictMock<INotificationService>();
            this.MockFor<INotificationService>().Setup(x => x.NotifyCustomerOfFailedOrder(testCustomer.CustomerId, NewOrderNumber));

            //Act
            await this.ClassUnderTest.CreateNewOrder(new List<OrderItem>(), testCustomer);

            //Assert
            this.VerifyAll();
            this.Verify<ILogger>(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [Test]
        public async Task CreateNewOrderCompletesSuccessfully()
        {
            //Arrange
            const bool ReserveProductIsSuccessful = true;
            var testSessionId = Guid.NewGuid();
            var testCustomer = new Customer
                               {
                                   CustomerId = 42
                               };
            var testOrder = new Order
                            {
                                Customer = testCustomer,
                                OrderNumber = 999
                            };

            this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));
            this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"Completed {nameof(OrderProcessor.CreateNewOrder)}"))));
            
            //
            //Use() - Use a specific mock rather than letting one be created automatically (just for added flexibility)
            //
            var mockOrderRepository = new Mock<IOrderRepository>();
            mockOrderRepository.Setup(x => x.SaveNewOrderAsync(It.IsAny<int>(), It.IsAny<List<OrderItem>>(), It.Is<Customer>(c => c.CustomerId == testCustomer.CustomerId)))
                               .ReturnsAsync(testOrder);
            this.Use(mockOrderRepository);

            this.MockFor<IInventoryService>().Setup(x => x.OpenSessionAsync())
                .ReturnsAsync(testSessionId);
            this.MockFor<IInventoryService>().Setup(x => x.TryReserveProductsAsync(It.IsAny<List<OrderItem>>()))
                .ReturnsAsync(ReserveProductIsSuccessful);
            this.MockFor<IInventoryService>().Setup(x => x.CommitSessionAsync(testSessionId))
                .Returns(Task.CompletedTask);

            this.MockFor<INotificationService>().Setup(x => x.NotifyCustomerOfSuccessfulOrder(testCustomer.CustomerId, testOrder.OrderNumber));

            //Act
            await this.ClassUnderTest.CreateNewOrder(new List<OrderItem>(), testCustomer);

            //Assert
            //
            //VerifyCallsFor() - Verifies calls for a specific mocked type; in this case, we could have also used VerifyAll() instead of making two calls to VerifyCallsFor()
            //
            this.VerifyCallsFor<IInventoryService>();
            this.VerifyCallsFor<INotificationService>();
            this.Verify<ILogger>(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never());
        }

        [Test]
        public async Task ReturnOrderItemLogsFailure()
        {
            //Arrange
            var testOrderItem = new OrderItem
                                {
                                    Quantity = 42,
                                    ProductId = 999
                                };
            var testCustomer = new Customer
            {
                CustomerId = 123,
            };

            this.MockFor<IInventoryService>().Setup(x => x.ReturnProductAsync(testOrderItem.ProductId, testOrderItem.Quantity))
                .ThrowsAsync(new ApplicationException("Returning Product has failed!"));

            this.MockFor<ILogger>().Setup(x => x.Error(It.Is<string>(m => m.Contains("error")), It.IsAny<ApplicationException>()));

            //Act
            await this.ClassUnderTest.ReturnOrderItem(testOrderItem, testCustomer);

            //Assert
            this.VerifyCallsFor<ILogger>();
            this.Verify<INotificationService>(x => x.NotifyCustomerOfReturnedProduct(testCustomer.CustomerId, testOrderItem.ProductId, testOrderItem.Quantity), Times.Never());
        }

        [Test]
        public async Task ReturnOrderItemCompletesSuccessfully()
        {
            //Arrange
            var testOrderItem = new OrderItem
                                {
                                    Quantity = 42,
                                    ProductId = 999
                                };
            var testCustomer = new Customer
                               {
                                   CustomerId = 768
                               };

            this.MockFor<IInventoryService>().Setup(x => x.ReturnProductAsync(testOrderItem.ProductId, testOrderItem.Quantity))
                .Returns(Task.CompletedTask);

            this.MockFor<INotificationService>().Setup(x => x.NotifyCustomerOfReturnedProduct(testCustomer.CustomerId, testOrderItem.ProductId, testOrderItem.Quantity));

            this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"Completed {nameof(OrderProcessor.ReturnOrderItem)}"))));

            //Act
            await this.ClassUnderTest.ReturnOrderItem(testOrderItem, testCustomer);

            //Assert
            this.VerifyAll();
        }
    }
}